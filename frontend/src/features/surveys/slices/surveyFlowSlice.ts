import { createSlice, PayloadAction } from '@reduxjs/toolkit';

interface Question {
    id: string;
    type: 'checkbox' | 'text';
    label: string;
    options?: string[];
}

interface SurveyFlowState {
    questions: Question[];
    currentQuestionIndex: number;
}

const initialState: SurveyFlowState = {
    questions: [
    ],
    currentQuestionIndex: 0,
};

const surveyFlowSlice = createSlice({
    name: 'surveyFlow',
    initialState,
    reducers: {
        nextQuestion: (state) => {
            if (state.currentQuestionIndex < state.questions.length - 1) {
                state.currentQuestionIndex += 1;
            } else {
                window.location.href = '/thank-you';
            }
        },
        resetSurvey: (state) => {
            state.currentQuestionIndex = 0;
        },
    },
});

export const { nextQuestion, resetSurvey } = surveyFlowSlice.actions;
export default surveyFlowSlice.reducer;
