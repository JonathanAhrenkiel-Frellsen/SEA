import {selectToken} from "../../auth/slices/authSlice";
import {ANALYSIS_API_URL} from "../../../shared/apiEndpoints";
import {store} from "../../../app/store";
import {SurveyCompletionRateDto} from "../../../shared/dto/SurveyCompletionRateDto";
import {SurveyResponseOverTimeDto} from "../../../shared/dto/SurveyResponseOverTimeDto";


export const fetchSurveyAnswersByUser = async (
    surveyId: string,
    page: number
): Promise<any[]> => {
    const jwt_token = selectToken(store.getState());
    if (!jwt_token) throw new Error("JWT token not available");

    const response = await fetch(
        `${ANALYSIS_API_URL}/surveyAnswers/${surveyId}?page=${page}`,
        {
            method: 'GET',
            headers: {
                'Authorization': `Bearer ${jwt_token}`,
                'Content-Type': 'application/json'
            }
        }
    );

    if (!response.ok) {
        const errorText = await response.text();
        throw new Error(`Failed to fetch survey answers: ${response.status} ${errorText}`);
    }

    return response.json();
};

export const fetchSurveyCompletionRate = async (
    surveyId: string
): Promise<SurveyCompletionRateDto> => {
    const jwt_token = selectToken(store.getState());
    if (!jwt_token) throw new Error("JWT token not available");

    const response = await fetch(
        `${ANALYSIS_API_URL}/surveyCompletionRate/${surveyId}`,
        {
            method: 'GET',
            headers: {
                'Authorization': `Bearer ${jwt_token}`,
                'Content-Type': 'application/json'
            }
        }
    );

    if (!response.ok) {
        const errorText = await response.text();
        throw new Error(`Failed to fetch completion rate: ${response.status} ${errorText}`);
    }

    return response.json();
};

export const fetchSurveyResponsesOverTime = async (
    surveyId: string
): Promise<SurveyResponseOverTimeDto[]> => {
    const jwt_token = selectToken(store.getState());
    if (!jwt_token) throw new Error("JWT token not available");

    const response = await fetch(
        `${ANALYSIS_API_URL}/surveyResponseOverTime/${surveyId}`,
        {
            method: 'GET',
            headers: {
                'Authorization': `Bearer ${jwt_token}`,
                    'Content-Type': 'application/json'
            }
        }
    );

    if (!response.ok) {
        const errorText = await response.text();
        throw new Error(`Failed to fetch responses over time: ${response.status} ${errorText}`);
    }

    return response.json();
};

