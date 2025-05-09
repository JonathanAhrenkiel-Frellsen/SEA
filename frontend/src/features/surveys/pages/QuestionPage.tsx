import CheckboxGroup from "../components/Inputs/CheckboxGroup";
import TextArea from "../components/Inputs/TextArea";
import { Button } from "../components/Buttons/Button";
import { ArrowRight } from "lucide-react";
import { useParams, useNavigate, useLocation } from "react-router-dom";
import { useEffect, useState } from "react";
import { fetchSurvey, loadSurveyAnswers, saveSurveyAnswer } from "../api/surveyApi";
import { DesignedSurveyDto, QuestionnaireDto } from "../../../shared/dto/DesignedSurveyDto";
import { SurveySaveAnswerDto } from "../../../shared/dto/SurveySaveAnswerDto";
import { resetSurveyAnswers, selectFieldValueById, setCheckboxValue, setTextValue } from "../slices/surveySlice";
import { store } from "../../../app/store";
import { useDispatch } from "react-redux";
import TextInput from "../components/Inputs/TextInput";

const QuestionPage = () => {
  const [questions, setQuestions] = useState<QuestionnaireDto[]>([]);
  const [title, setTitle] = useState<string>('');
  const [currentQuestionIndex, setCurrentQuestionIndex] = useState(0);
  const [pin, setPin] = useState('');
  const [isPinRequired, setIsPinRequired] = useState(false);
  const [pinError, setPinError] = useState('');
  const { id } = useParams<{ id?: string }>();
  const location = useLocation();
  const navigate = useNavigate();
  const dispatch = useDispatch();

  const searchParams = new URLSearchParams(location.search);

  const handleContinue = () => {
    const question = questions[currentQuestionIndex];
    if (!question) return;

    const answer = selectFieldValueById(store.getState().surveyForm, question.QuestionnaireId.toString());

    const isAnswerEmpty =
      question.InputType === 'text'
        ? !answer || answer.trim() === ''
        : !answer || (Array.isArray(answer) && answer.length === 0);

    if (isAnswerEmpty) {
      alert("Please answer the question before continuing.");
      return;
    }

    const surveyAnswer: SurveySaveAnswerDto = {
      SurveyId: id ? parseInt(id) : 0,
      QuestionnaireId: question.QuestionnaireId,
      SurveyAnswer: Array.isArray(answer) ? answer.join(', ') : answer
    };

    saveSurveyAnswer(surveyAnswer).then(() => {
      if (currentQuestionIndex >= questions.length - 1) {
        navigate(`/thank-you`);
        return;
      }

      setCurrentQuestionIndex((prev) => prev + 1);
    });
  };

  const handlePinSubmit = () => {
    if (!/^\d+$/.test(pin)) {
      setPinError("PIN must be numbers only.");
      return;
    }

    if (!id) return;

    fetchSurvey(id, pin)
      .then((survey: DesignedSurveyDto) => {
        if (!survey.Questionnaires) {
          setPinError("Invalid or expired survey.");
          return;
        }

        setTitle(survey.SurveyTitle ?? '');
        setQuestions(survey.Questionnaires);
        dispatch(resetSurveyAnswers());
        setIsPinRequired(false); // Hide PIN UI
        setPinError("");

        loadSurveyAnswers(id, pin).then((answers) => {
          if (!answers.SurveyStoredAnwsers) return;

          for (const answer of answers.SurveyStoredAnwsers) {
            if (answer.InputType === 'text') {
              dispatch(setTextValue({ name: answer.QuestionnaireId!.toString(), value: answer.SurveyAnswer! }));
            }
            else if (answer.InputType === 'checkbox') {
              const selectedValues = answer.SurveyAnswer?.split(', ') || [];
              dispatch(setCheckboxValue({ name: answer.QuestionnaireId!.toString(), value: selectedValues }));
            }
          }

          const questionIndex = answers.SurveyStoredAnwsers.filter(survey => survey.SurveyAnswer !== "").length;
          if (questionIndex > answers.SurveyStoredAnwsers.length - 1) {
            navigate(`/thank-you`);
            return;
          }

          setCurrentQuestionIndex(questionIndex);
        });
      })
      .catch(() => {
        setPinError("Invalid PIN.");
      });
  };

  useEffect(() => {
    const requiresPin = searchParams.get("pinCode") === "true";
    setIsPinRequired(requiresPin);

    if (!requiresPin && id) {
      fetchSurvey(id, '')
        .then((survey: DesignedSurveyDto) => {
          if (!survey.Questionnaires) {
            console.error("No questions found in the survey.");
            return;
          }

          setQuestions(survey.Questionnaires);
          setTitle(survey.SurveyTitle ?? '');

          dispatch(resetSurveyAnswers());

          loadSurveyAnswers(id, pin).then((answers) => {
            if (!answers.SurveyStoredAnwsers) return;

            for (const answer of answers.SurveyStoredAnwsers) {
              if (answer.InputType === 'text') {
                dispatch(setTextValue({ name: answer.QuestionnaireId!.toString(), value: answer.SurveyAnswer! }));
              }
              else if (answer.InputType === 'checkbox') {
                const selectedValues = answer.SurveyAnswer?.split(', ') || [];
                dispatch(setCheckboxValue({ name: answer.QuestionnaireId!.toString(), value: selectedValues }));
              }
            }

            const questionIndex = answers.SurveyStoredAnwsers.filter(survey => survey.SurveyAnswer !== "").length;
            if (questionIndex > answers.SurveyStoredAnwsers.length - 1) {
              navigate(`/thank-you`);
              return;
            }

            setCurrentQuestionIndex(questionIndex);
          });
        });
    }
  }, [id]);

  if (isPinRequired) {
    return (
      <div className="p-6 max-w-md mx-auto">
        <h1 className="text-xl font-bold mb-4">Enter PIN Code</h1>
        <TextInput value={pin} setValue={setPin} />
        {pinError && <p className="text-red-500 mb-2">{pinError}</p>}
        <Button text="Submit PIN" type="primary" onClick={handlePinSubmit} />
      </div>
    );
  }

  return (
    <div>
      {questions[currentQuestionIndex] && (
        <h1 className={'text-3xl font-bold mb-4'}>{title}</h1>
      )}

      {questions[currentQuestionIndex]?.InputType === 'checkbox' &&
        questions[currentQuestionIndex]?.MultipleChoices && (
          <CheckboxGroup
            key={questions[currentQuestionIndex].QuestionnaireId}
            name={questions[currentQuestionIndex].QuestionnaireId.toString()}
            options={questions[currentQuestionIndex].MultipleChoices}
          />
        )}

      {questions[currentQuestionIndex]?.InputType === 'text' && (
        <TextArea
          key={questions[currentQuestionIndex].QuestionnaireId}
          name={questions[currentQuestionIndex].QuestionnaireId.toString()}
          label={questions[currentQuestionIndex].QuestionnaireTitle}
          placeholder={'Type your answer here'}
        />
      )}

      <div className={'flex justify-end mt-10'}>
        <Button
          text={'Continue'}
          type={"primary"}
          onClick={handleContinue}
          icon={<ArrowRight size={16} />}
        />
      </div>
    </div>
  );
};

export default QuestionPage;
