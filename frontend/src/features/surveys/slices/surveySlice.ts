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
            action: PayloadAction<{ name: string; value: string }>
        ) => {
            const { name, value } = action.payload;
            if (!state[name]) state[name] = [];

            if (state[name].includes(value)) {
                state[name] = (state[name] as string[]).filter((v) => v !== value);
            } else {
                (state[name] as string[]).push(value);
            }
        },
        setTextValue: (
            state,
            action: PayloadAction<{ name: string; value: string }>
        ) => {
            state[action.payload.name] = action.payload.value;
        },
    },
});

export const { setCheckboxValue, setTextValue } = surveySlice.actions;
export default surveySlice.reducer;
