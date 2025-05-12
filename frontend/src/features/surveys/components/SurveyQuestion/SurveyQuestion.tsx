import CheckboxGroup from "../Inputs/CheckboxGroup";
import TextArea from "../Inputs/TextArea";
import { QuestionnaireDto } from "../../../../shared/dto/DesignedSurveyDto";

interface SurveyQuestionProps {
    question: QuestionnaireDto;
    title: string;
}

const SurveyQuestion = ({ question, title }: SurveyQuestionProps) => {
    return (
        <div>
            <h1 className="text-3xl font-bold mb-4">{title}</h1>

            {question.InputType === 'checkbox' && question.MultipleChoices && (
                <CheckboxGroup
                    key={question.QuestionnaireId}
                    name={question.QuestionnaireId.toString()}
                    label={question.QuestionnaireTitle}
                    options={question.MultipleChoices}
                />
            )}

            {question.InputType === 'text' && (
                <TextArea
                    key={question.QuestionnaireId}
                    name={question.QuestionnaireId.toString()}
                    label={question.QuestionnaireTitle}
                    placeholder="Type your answer here"
                />
            )}
        </div>
    );
};

export default SurveyQuestion;
