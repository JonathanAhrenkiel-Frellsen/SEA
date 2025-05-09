import CheckboxGroup from "../components/Inputs/CheckboxGroup";
import TextArea from "../components/Inputs/TextArea";
import {Button} from "../components/Buttons/Button";
import {ArrowRight} from "lucide-react";
import {useParams} from "react-router-dom";
import {useEffect, useState} from "react";
import {fetchSurvey, loadSurveyAnswers, saveSurveyAnswer} from "../api/surveyApi";
import {DesignedSurveyDto, QuestionnaireDto} from "../../../shared/dto/DesignedSurveyDto";
import {SurveySaveAnswerDto} from "../../../shared/dto/SurveySaveAnswerDto";
import {resetSurveyAnswers, selectFieldValueById, setCheckboxValue, setTextValue} from "../slices/surveySlice";
import {store} from "../../../app/store";
import {useNavigate} from "react-router-dom";
import {ExperimenteeAppDto} from "../../../shared/dto/ExperimenteeAppDto";
import {useDispatch} from "react-redux";

const QuestionPage = () => {
    const [questions, setQuestions] = useState<QuestionnaireDto[]>([]);
    const [title, setTitle] = useState<string>('');
    const [currentQuestionIndex, setCurrentQuestionIndex] = useState(0);
    const { id } = useParams<{ id?: string }>();
    const navigate = useNavigate();
    const dispatch = useDispatch();

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

    useEffect(() => {
        if (!id) return;

        fetchSurvey(id).then((survey: DesignedSurveyDto) => {
            if (!survey.Questionnaires) {
                console.error("No questions found in the survey.");
                return;
            }

            setCurrentQuestionIndex(0);

            setQuestions(survey.Questionnaires);
            setTitle(survey.SurveyTitle ?? '');
        })

        dispatch(resetSurveyAnswers())

        loadSurveyAnswers(id).then((answers) => {
            if (!answers.SurveyStoredAnwsers) {
                console.error("No stored answers found.");
                return;
            }

            for (const answer of answers.SurveyStoredAnwsers) {
                if (answer.InputType === 'text') {
                    dispatch(setTextValue({ name: answer.QuestionnaireId!.toString(), value: answer.SurveyAnswer! }));
                }
                else if (answer.InputType === 'checkbox') {
                    if (!answer.SurveyAnswer) {
                        continue;
                    }

                    const selectedValues = answer.SurveyAnswer?.split(', ') || [];
                    dispatch(setCheckboxValue({ name: answer.QuestionnaireId!.toString(), value: selectedValues }));
                }
            }

            const questionIndex = answers.SurveyStoredAnwsers.filter(survey => survey.SurveyAnswer !== "").length;

            if (questionIndex >= answers.SurveyStoredAnwsers.length - 1) {
                navigate(`/thank-you`);
                return;
            }

            setCurrentQuestionIndex(questionIndex)
        });
    }, [id]);

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
