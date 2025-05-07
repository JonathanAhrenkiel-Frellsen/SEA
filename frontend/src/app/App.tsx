import React from 'react';
import { BrowserRouter as Router } from 'react-router-dom';
import { AppRoutes } from './routes';
import Header from "../features/surveys/components/Header/Header";

const App = () => {
    return (
        <>
          <Router>
            <Header/>
            <div className={'w-full max-w-[1000px] sm:w-[90%] mx-auto mt-10'}>
              <AppRoutes />
            </div>

          </Router>
        </>
    );
};

export default App;
