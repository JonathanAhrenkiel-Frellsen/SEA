import { configureStore, combineReducers } from '@reduxjs/toolkit';
import storage from 'redux-persist/lib/storage';
import { persistReducer, persistStore } from 'redux-persist';

import surveyReducer from '../features/surveys/slices/surveySlice';
import surveyFlowReducer from '../features/surveys/slices/surveyFlowSlice';
import authReducer from '../features/auth/slices/authSlice';

const rootReducer = combineReducers({
    surveyForm: surveyReducer,
    surveyFlow: surveyFlowReducer,
    auth: authReducer,
});

const persistConfig = {
    key: 'root',
    storage,
};

const persistedReducer = persistReducer(persistConfig, rootReducer);

export const store = configureStore({
    reducer: persistedReducer,
    middleware: getDefaultMiddleware =>
      getDefaultMiddleware({
          serializableCheck: false, // required for redux-persist
      }),
});

export const persistor = persistStore(store);

export type RootState = ReturnType<typeof store.getState>;
export type AppDispatch = typeof store.dispatch;
