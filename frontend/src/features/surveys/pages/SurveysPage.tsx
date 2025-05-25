import React, { useState } from 'react';
import Papa from 'papaparse';
import { useNavigate } from 'react-router-dom';
import { StartSurveyModal } from '../components/Modals/StartSurveyModal';

const SurveysPage = () => {
    const navigate = useNavigate();
    const [showModal, setShowModal] = useState(false);

    const handleImportSurveySuccess = (data: any[]) => {
        setShowModal(false); // Luk modal før navigation
        navigate('/surveys/create', { state: { importedSurvey: data } });
    };

    const handleImportSurvey = (e: React.ChangeEvent<HTMLInputElement>) => {
        const file = e.target.files?.[0];
        if (!file) return;

        Papa.parse(file, {
            header: true,
            skipEmptyLines: true,
            complete: (results) => {
                const data = results.data as any[];
                const cleanedData = data.filter(row =>
                    row['Survey Title'] &&
                    row['Accessibility'] &&
                    row['Question'] &&
                    row['Answer Type']
                );
                if (cleanedData.length !== data.length) {
                    alert("Nogle rækker i CSV mangler påkrævede felter og er blevet ignoreret.");
                }
                if (!cleanedData.length) {
                    alert("CSV-filen indeholder ingen gyldige data.");
                    return;
                }
                handleImportSurveySuccess(cleanedData);
            },
            error: (err) => {
                alert('Failed to parse CSV: ' + err.message);
            }
        });
        e.target.value = '';
    };

    const handleFreshSurvey = () => {
        setShowModal(false);
        navigate('/surveys/create'); // <-- ikke /surveys/new
    };

    const handleCloseModal = () => {
        setShowModal(false);
    };

    return (
        <div>
            {/* ...din øvrige side... */}
            <button onClick={() => setShowModal(true)}>Make new survey</button>
            {showModal && (
                <StartSurveyModal
                    onFreshSurvey={handleFreshSurvey}
                    onImportSurvey={handleImportSurvey}
                    onClose={handleCloseModal}
                />
            )}
        </div>
    );
};

export default SurveysPage;