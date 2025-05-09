import { createSlice, PayloadAction } from '@reduxjs/toolkit';

interface SurveyState {
    [key: string]: string[] | string;
}

const initialState: SurveyState = {};

const surveySlice = createSlice({
    name: 'survey',
    initialState,
    reducers: {
        setCheckboxValue: (
            state,
            action: PayloadAction<{ name: string; value: string | string[] }>
        ) => {
            const { name, value } = action.payload;
            if (!state[name]) state[name] = [];

            if (typeof value === 'string') {
                if (state[name].includes(value)) {
                    state[name] = (state[name] as string[]).filter((v) => v !== value);
                } else {
                    (state[name] as string[]).push(value);
                }
            } else {
                state[name] = value;
            }
        },
        setTextValue: (
            state,
            action: PayloadAction<{ name: string; value: string }>
        ) => {
            state[action.payload.name] = action.payload.value;
        },
        resetSurveyAnswers: (state) => {
            Object.keys(state).forEach(key => delete state[key]);
        }
    },
});

export const selectFieldValueById = (state: SurveyState, id: string): string => {
    const value = state[id];
    if (Array.isArray(value)) {
        return value.join(', ');
    }
    return value ?? '';
};

export const { setCheckboxValue, setTextValue, resetSurveyAnswers } = surveySlice.actions;
export default surveySlice.reducer;
