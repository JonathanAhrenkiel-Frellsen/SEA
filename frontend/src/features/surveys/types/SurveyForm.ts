import {QuestionnaireDto} from "../../../shared/dto/DesignedSurveyDto";

export interface SurveyForm {
    title: string;
    startDate: string | null;
    endDate: string | null;
    isPrivate: boolean;
    questions: QuestionnaireDto[];
}
