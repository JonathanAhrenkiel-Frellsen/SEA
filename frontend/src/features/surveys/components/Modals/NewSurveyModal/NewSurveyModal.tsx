import { X, ArrowRight, Upload } from 'lucide-react';
import { Button } from '../../../../../shared/components/Buttons/Button';

interface NewSurveyModalProps {
    onClose: () => void;
}

const NewSurveyModal: React.FC<NewSurveyModalProps> = ({ onClose }) => {
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

                <Button text="Fresh Survey" icon={<ArrowRight size={16} />} type="primary" onClick={() => window.location.href = '/surveys/new'} />

                <Button text="Import Survey" icon={<Upload size={16} />} type="secondary" onClick={() => window.location.href = '/surveys/new'} />
            </div>
        </div>
    );
};

export default NewSurveyModal;
