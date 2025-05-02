import { configureStore } from '@reduxjs/toolkit';
import surveyReducer from '../features/surveys/slices/surveySlice';
import surveyFlowReducer from '../features/surveys/slices/surveyFlowSlice';


export const store = configureStore({
    reducer: {
        surveyForm: surveyReducer,
        surveyFlow: surveyFlowReducer,
    },
});

export type RootState = ReturnType<typeof store.getState>;
export type AppDispatch = typeof store.dispatch;
