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
import { completeSurvey, saveSurveyAnswer } from "../api/surveyApi";
import { Button } from "../../../shared/components/Buttons/Button";
import { ArrowRight } from "lucide-react";
import PinEntryForm from "../components/PinEntryForm/EntryEntryForm";
import SurveyQuestion from "../components/SurveyQuestion/SurveyQuestion";

const QuestionPage: React.FC = () => {
  const { id } = useParams<{ id?: string }>();
  const location = useLocation();
  const navigate = useNavigate();
  const dispatch = useDispatch();
  const searchParams = new URLSearchParams(location.search);

  // State
  const [questions, setQuestions]               = useState<QuestionnaireDto[]>([]);
  const [title, setTitle]                       = useState<string>("");
  const [currentQuestionIndex, setCurrentQuestionIndex] = useState(0);
  const [pin, setPin]                           = useState<string>("");
  const [isPinRequired, setIsPinRequired]       = useState<boolean>(false);
  const [pinError, setPinError]                 = useState<string>("");
  const [isPaused, setIsPaused]                 = useState<boolean>(false);
  const [surveyClosed, setSurveyClosed]         = useState<boolean>(false);
  const [errorMessage, setErrorMessage]         = useState<string>("");

  // Fetch and populate survey answers + pause flag
  async function loadSurvey() {
    // reset any previous errors
    setSurveyClosed(false);
    setErrorMessage("");
    setPinError("");
    // PIN validation
    if (isPinRequired && !/^\d+$/.test(pin)) {
      setPinError("PIN must be numbers only.");
      return;
    }
    if (!id) return;

    let result;
    try {
      result = await loadParsedSurveyAnswers(id, pin);
    } catch (err: any) {
      const status = err.response?.status;
      // 403 Forbidden usually means “invalid or missing PIN”
      if (status === 403) {                                   // ← CHANGED
        setPinError("Invalid or missing PIN.");               // ← CHANGED
        return;                                               // ← CHANGED
      }

     

      // catch paused‐survey 400
      if (status === 400) {                                   // ← CHANGED
        setErrorMessage(err.response.data || "Survey is closed.");
        setSurveyClosed(true);
        return;
      }

      // ANY other error (404, 500, etc.)
      setErrorMessage("Something went wrong. Please try again later."); // ← CHANGED
      setSurveyClosed(true);                                             // ← CHANGED
      return;
    }

    if (!result) return;

    // populate state
    setIsPaused(result.isPaused);
    setTitle(result.title);
    setQuestions(result.questions.sort((a, b) => a.QuestionnairePos - b.QuestionnairePos));

    // restore saved answers
    result.answers.forEach(answer => {
      if (answer.type === "text") {
        dispatch(setTextValue({ name: answer.id, value: answer.value as string }));
      } else {
        dispatch(setCheckboxValue({ name: answer.id, value: answer.value as string[] }));
      }
    });

    // if already complete, redirect
    if (result.isComplete) {
      navigate("/thank-you");
    } else {
      setCurrentQuestionIndex(result.nextIndex);
    }

    // hide PIN-entry on success
    setIsPinRequired(false);
  }

  // Submit a single answer
  const handleContinue = () => {
    if (isPaused) return;           // extra guard
    const question = questions[currentQuestionIndex];
    if (!question) return;

    const answer = selectFieldValueById(
        store.getState().surveyForm,
        question.QuestionnaireId.toString(),
    );

    const isEmpty =
        question.InputType === "text"
            ? !answer || answer.trim() === ""
            : !answer || (Array.isArray(answer) && answer.length === 0);

    if (isEmpty) {
      alert("Please answer before continuing.");
      return;
    }

    const dto: SurveySaveAnswerDto = {
      SurveyId: id ? parseInt(id) : 0,
      QuestionnaireId: question.QuestionnaireId,
      SurveyAnswer: Array.isArray(answer) ? answer.join(", ") : answer,
    };

    saveSurveyAnswer(dto).then(() => {
      if (currentQuestionIndex >= questions.length - 1) {
        completeSurvey(id!).then(() => navigate("/thank-you"));
      } else {
        setCurrentQuestionIndex(prev => prev + 1);
      }
    });
  };

  useEffect(() => {
    const requiresPin = searchParams.get("pinCode") === "true";
    setIsPinRequired(requiresPin);
    if (!requiresPin && id) {
      loadSurvey();
    }
  }, [id]);

  if (surveyClosed) {
    return (
        <div className="flex items-center justify-center h-full p-8">
          <div className="bg-yellow-100 border-l-4 border-yellow-500 p-4 max-w-md text-yellow-800">
            <p className="font-bold">Survey not available</p>
            <p className="mt-2">{errorMessage}</p>
          </div>
        </div>
    );
  }




  if (isPaused) {
    return (
        <div className="p-6 text-center text-lg">
          This survey is paused
        </div>
    );
  }


  if (isPinRequired) {
    return (
        <PinEntryForm pin={pin} setPin={setPin} pinError={pinError} onSubmit={loadSurvey} />
    );
  }

  const question = questions[currentQuestionIndex];
  return (
      <div>
        {question && (
            <>
              <SurveyQuestion question={question} title={title} />
              <div className="flex flex-col items-end mt-10">
                <Button
                    text="Continue"
                    type="primary"
                    onClick={handleContinue}
                    icon={<ArrowRight size={16} />}
                    disabled={isPaused}
                />
                {isPaused && (
                    <p className="text-red-500 mt-2">
                      This survey has been paused. You cannot submit answers right now.
                    </p>
                )}
              </div>
            </>
        )}
      </div>
  );
};

export default QuestionPage;
