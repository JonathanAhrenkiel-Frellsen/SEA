import { X, ArrowRight, Upload } from 'lucide-react';
import { Button } from '../../../../../shared/components/Buttons/Button';
import React, { useRef } from 'react';
import Papa from 'papaparse';
import { useNavigate } from 'react-router-dom';

interface NewSurveyModalProps {
    onClose: () => void;
}

const NewSurveyModal: React.FC<NewSurveyModalProps> = ({ onClose }) => {
    const fileInputRef = useRef<HTMLInputElement>(null);
    const navigate = useNavigate();

    const handleImportClick = () => {
        fileInputRef.current?.click();
    };

    const handleFileChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        const file = e.target.files?.[0];
        if (!file) return;

        Papa.parse(file, {
            header: true,
            skipEmptyLines: true,
            complete: (results) => {
                const data = results.data as any[];
                const meta = results.meta as Papa.ParseMeta;
                const expectedHeaders = [
                    "Survey Title",
                    "Accessibility",
                    "Question",
                    "Answer Type",
                    "Answer Options"
                ];
                const actualHeaders = meta.fields || [];
                const isValidHeader =
                    expectedHeaders.every((h, i) => h === actualHeaders[i]) &&
                    actualHeaders.length === expectedHeaders.length;
                if (!isValidHeader) {
                    alert("CSV header skal være præcis: Survey Title,Accessibility,Question,Answer Type,Answer Options");
                    return;
                }
                if (!data.length) {
                    alert("CSV-filen indeholder ingen data.");
                    return;
                }
                navigate('/surveys/create', { state: { importedSurvey: data } });
            },
            error: (err) => {
                alert('Failed to parse CSV: ' + err.message);
            }
        });
        e.target.value = '';
    };

    return (
        <div className="fixed inset-0 bg-black bg-opacity-60 flex items-center justify-center z-50">
            <div className="bg-main p-6 text-white font-josefin relative w-80 flex flex-col gap-4">
                <button
                    onClick={onClose}
                    className="absolute top-3 right-3 text-white hover:opacity-75"
                >
                    <X />
                </button>

                <h2 className="text-lg text-center font-semibold">How would you like to start?</h2>

                <Button
                    text="Fresh Survey"
                    icon={<ArrowRight size={16} />}
                    type="primary"
                    onClick={() => navigate('/surveys/new')}
                />

                <Button
                    text="Import Survey"
                    icon={<Upload size={16} />}
                    type="secondary"
                    onClick={handleImportClick}
                />
                <input
                    type="file"
                    accept=".csv"
                    ref={fileInputRef}
                    style={{ display: 'none' }}
                    onChange={handleFileChange}
                />
            </div>
        </div>
    );
};

export default NewSurveyModal;
