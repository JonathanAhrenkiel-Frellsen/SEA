import {loadSurveyAnswers} from "../api/surveyApi";
import {QuestionnaireDto} from "../../../shared/dto/DesignedSurveyDto";
import { ExperimenteeAppDto }    from '../../../shared/dto/ExperimenteeAppDto';
export interface ParsedSurveyAnswers {
    title: string;
    questions: QuestionnaireDto[];
    answers: {
        id: string;
        type: 'text' | 'checkbox';
        value: string | string[];
    }[];
    nextIndex: number;
    isComplete: boolean;
    isPaused: boolean;
}

export async function loadParsedSurveyAnswers(
    id: string,
    pin: string
): Promise<ParsedSurveyAnswers | null> {
    const answers = await loadSurveyAnswers(id, pin);
    if (!answers?.SurveyStoredAnwsers) return null;

    const title = answers.SurveyTitle ?? '';
    const questions = answers.SurveyStoredAnwsers.map(answer => ({
        QuestionnaireId: answer.QuestionnaireId!,
        QuestionnairePos: answer.QuestionnairePos,
        QuestionnaireTitle: answer.QuestionnaireTitle!,
        InputType: answer.InputType!,
        Range: answer.Range!,
        SurveyId: id,
        MultipleChoices: answer.MultipleChoices
    } as unknown as QuestionnaireDto));

    const parsedAnswers = answers.SurveyStoredAnwsers.map(answer => {
        if (answer.InputType === 'text') {
            return {
                id: answer.QuestionnaireId!.toString(),
                type: 'text',
                value: answer.SurveyAnswer!
            };
        } else if (answer.InputType === 'checkbox') {
            const selectedValues = answer.SurveyAnswer?.split(', ') || [];
            return {
                id: answer.QuestionnaireId!.toString(),
                type: 'checkbox',
                value: selectedValues
            };
        }
        return null;
    }).filter(Boolean) as ParsedSurveyAnswers['answers'];

    const nextIndex = answers.SurveyStoredAnwsers.filter(s => s.SurveyAnswer !== "").length;
    const isComplete = nextIndex > answers.SurveyStoredAnwsers.length - 1;

    return {
        title,
        questions,
        answers: parsedAnswers,
        nextIndex,
        isComplete,
        isPaused: answers.IsPaused
    };
}
