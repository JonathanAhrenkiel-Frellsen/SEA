import {QuestionnaireDto} from "../../../shared/dto/DesignedSurveyDto";

export interface SurveyForm {
    title: string;
    isPrivate: boolean;
    questions: QuestionnaireDto[];
}