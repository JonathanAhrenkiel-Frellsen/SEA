import React, { useRef } from 'react';

interface StartSurveyModalProps {
    onFreshSurvey: () => void;
    onImportSurvey: (e: React.ChangeEvent<HTMLInputElement>) => void;
    onClose: () => void;
}

export const StartSurveyModal: React.FC<StartSurveyModalProps> = ({
    onFreshSurvey,
    onImportSurvey,
    onClose,
}) => {
    const fileInputRef = useRef<HTMLInputElement>(null);

    const handleImportClick = () => {
        fileInputRef.current?.click();
    };

    return (
        <div className="modal">
            <button onClick={onClose}>×</button>
            <h2>How would you like to start?</h2>
            <button onClick={onFreshSurvey}>Fresh Survey →</button>
            <button onClick={handleImportClick}>Import Survey ⬆️</button>
            <input
                type="file"
                accept=".csv"
                ref={fileInputRef}
                style={{ display: 'none' }}
                onChange={onImportSurvey}
            />
        </div>
    );
};
