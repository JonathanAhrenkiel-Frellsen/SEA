import React from "react";
import { PartyPopper } from 'lucide-react';

const FinishSurveyPage = () => {
    return (
        <div className="flex flex-col items-center justify-center bg-main text-text px-4 text-center font-josefin">
            <PartyPopper className="w-16 h-16 mb-6" />
            <h1 className="text-4xl font-bold mb-4">Thank You!</h1>
            <p className="text-lg max-w-md">
                We appreciate you taking the time to complete our survey. Your responses help us improve.
            </p>
        </div>
    );
};

export default FinishSurveyPage;
