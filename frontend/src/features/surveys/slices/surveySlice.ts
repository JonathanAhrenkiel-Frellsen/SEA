import { createSlice } from "@reduxjs/toolkit";

const surveySlice = createSlice({
    name: 'surveys',
    initialState: [],
    reducers: {
        addSurvey: (state, action) => {
            console.log(action.payload);
        },
    },
});
