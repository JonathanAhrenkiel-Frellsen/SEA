import React from 'react';
import { BrowserRouter as Router } from 'react-router-dom';
import { AppRoutes } from './routes';
import Header from "../features/surveys/components/Header/Header";

const App = () => {
    return (
        <>
            <Header/>
            <div className={'w-full max-w-[1000px] sm:w-[90%] mx-auto mt-10'}>
                <Router>
                    <AppRoutes />
                </Router>
            </div>
        </>
    );
};

export default App;
