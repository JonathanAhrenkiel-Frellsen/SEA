import { DesignedSurveyDto } from '../../../../../shared/dto/DesignedSurveyDto';
import { Button } from '../../../../../shared/components/Buttons/Button';
import CopyBox from '../../../../../shared/components/CopyBox/CopyBox';

interface SuccessModalProps {
    surveyResponse: DesignedSurveyDto;
    onClose: () => void;
}

const SuccessModal = ({ surveyResponse, onClose }: SuccessModalProps) => {
    const surveyUrl = `${window.location.origin}/${surveyResponse.SurveyId}/questions?pinCode=${surveyResponse.PrivateKey !== ''}`;
    const hasPrivateKey = surveyResponse.PrivateKey && surveyResponse.PrivateKey !== '';

    return (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
            <div className="bg-main text-white p-6 w-96 text-center rounded shadow-xl">
                <h2 className="text-2xl font-semibold mb-4">Survey Published!</h2>
                <p className="mb-6">Your survey was published. You can now share the survey link.</p>

                <CopyBox label="Shareable Link:" value={surveyUrl} />

                {hasPrivateKey && (
                    <CopyBox label="Private Key:" value={surveyResponse.PrivateKey ?? ''} />
                )}

                <div className="float-end mt-4">
                    <Button text="Go to Surveys" type="primary" onClick={onClose} />
                </div>
            </div>
        </div>
    );
};

export default SuccessModal;
