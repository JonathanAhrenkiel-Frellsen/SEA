import { useSelector, useDispatch } from 'react-redux';
import CheckboxGroup from "../components/Inputs/CheckboxGroup";
import TextArea from "../components/Inputs/TextArea";
import {Button} from "../components/Buttons/Button";
import {ArrowRight} from "lucide-react";
import {RootState} from "../../../app/store";
import {nextQuestion} from "../slices/surveyFlowSlice";

const QuestionPage = () => {
    const dispatch = useDispatch();
    const { questions, currentQuestionIndex } = useSelector((state: RootState) => state.surveyFlow);
    const currentQuestion = questions[currentQuestionIndex];

    const handleContinue = () => {
        // Optional: Validate input before continuing
        dispatch(nextQuestion());
    };

    return (
        <div>
            <h1 className={'text-3xl font-bold mb-4'}>{currentQuestion.label}</h1>

            {currentQuestion.type === 'checkbox' && currentQuestion.options && (
                <CheckboxGroup
                    name={currentQuestion.id}
                    options={currentQuestion.options}
                />
            )}
            {currentQuestion.type === 'text' && (
                <TextArea
                    name={currentQuestion.id}
                    label={currentQuestion.label}
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
