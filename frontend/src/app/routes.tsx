import React from 'react';
import { Route, Routes } from 'react-router-dom';

import LoginPage from '../features/surveys/pages/LoginPage';
import QuestionPage from '../features/surveys/pages/QuestionPage';
import SurveyListPage from '../features/surveys/pages/SurveyListPage';
import SurveyFormPage from '../features/surveys/pages/SurveyEditorPage';
import AnalysisPage from '../features/surveys/pages/AnalysisPage';
import FinishSurveyPage from '../features/surveys/pages/FinishSurveyPage';

export const AppRoutes = () => {
    return (
        <Routes>
            <Route path="/login" element={<LoginPage />} />
            <Route path="/:id/questions" element={<QuestionPage />} />
            <Route path="/thank-you" element={<FinishSurveyPage />} />
            <Route path="/surveys" element={<SurveyListPage />} />
            <Route path="/surveys/new" element={<SurveyFormPage />} />
            <Route path="/surveys/:id/edit" element={<SurveyFormPage />} />
            <Route path="/analysis/:id" element={<AnalysisPage />} />
        </Routes>
    );
};
