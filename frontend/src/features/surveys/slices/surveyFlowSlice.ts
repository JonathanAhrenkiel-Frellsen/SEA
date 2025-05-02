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
        { id: 'q1', type: 'checkbox', label: 'Pick your favorite fruits', options: ['Apple', 'Banana', 'Orange'] },
        { id: 'q2', type: 'text', label: 'Tell us why you chose those fruits' },
        // add more questions here
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
                // redirect to /thank-you
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
