import { useParams, useNavigate, useLocation } from "react-router-dom";
import { useEffect, useState } from "react";
import { useDispatch } from "react-redux";
import { QuestionnaireDto } from "../../../shared/dto/DesignedSurveyDto";
import { SurveySaveAnswerDto } from "../../../shared/dto/SurveySaveAnswerDto";
import { store } from "../../../app/store";
import {
  setCheckboxValue,
  setTextValue,
  selectFieldValueById,
} from "../slices/surveySlice";
import { loadParsedSurveyAnswers } from "../services/surveyService";
import {completeSurvey, saveSurveyAnswer} from "../api/surveyApi";
import {Button} from "../../../shared/components/Buttons/Button";
import {ArrowRight} from "lucide-react";
import PinEntryForm from "../components/PinEntryForm/EntryEntryForm";
import SurveyQuestion from "../components/SurveyQuestion/SurveyQuestion";

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

    const answer = selectFieldValueById(
        store.getState().surveyForm,
        question.QuestionnaireId.toString()
    );

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
      SurveyAnswer: Array.isArray(answer) ? answer.join(', ') : answer,
    };

    saveSurveyAnswer(surveyAnswer).then(() => {
      if (currentQuestionIndex >= questions.length - 1) {
        completeSurvey(id!).then(() => {
          navigate('/thank-you');
          return;
        })
      }

      setCurrentQuestionIndex((prev) => prev + 1);
    });
  };

  const loadSurvey = async () => {
    if (isPinRequired && !/^\d+$/.test(pin)) {
      setPinError("PIN must be numbers only.");
      return;
    }

    if (!id) return;

    const result = await loadParsedSurveyAnswers(id, pin);
    if (!result) return;

    setTitle(result.title);
    setQuestions(result.questions.sort((a, b) => a.QuestionnairePos - b.QuestionnairePos));

    result.answers.forEach((answer) => {
      if (answer.type === 'text') {
        dispatch(setTextValue({ name: answer.id, value: answer.value as string }));
      } else if (answer.type === 'checkbox') {
        dispatch(setCheckboxValue({ name: answer.id, value: answer.value as string[] }));
      }
    });

    if (result.isComplete) {
      navigate('/thank-you');
    } else {
      setCurrentQuestionIndex(result.nextIndex);
    }

    setIsPinRequired(false);
  };

  useEffect(() => {
    const requiresPin = searchParams.get("pinCode") === "true";
    setIsPinRequired(requiresPin);

    if (!requiresPin && id) {
      loadSurvey();
    }
  }, [id]);

  if (isPinRequired) {
    return (
        <PinEntryForm
            pin={pin}
            setPin={setPin}
            pinError={pinError}
            onSubmit={loadSurvey}
        />
    );
  }

  const currentQuestion = questions[currentQuestionIndex];

  return (
      <div>
        {currentQuestion && (
            <>
              <SurveyQuestion question={currentQuestion} title={title} />
              <div className="flex justify-end mt-10">
                <Button
                    text="Continue"
                    type="primary"
                    onClick={handleContinue}
                    icon={<ArrowRight size={16} />}
                />
              </div>
            </>
        )}
      </div>
  );
};

export default QuestionPage;
