import { DesignedSurveyDto } from '../../../../../shared/dto/DesignedSurveyDto';
import { Button } from '../../../../../shared/components/Buttons/Button';
import CopyBox from '../../../../../shared/components/CopyBox/CopyBox';

interface SuccessModalProps {
    surveyResponse: DesignedSurveyDto;
    onClose: () => void;
}

const SuccessModal = ({ surveyResponse, onClose }: SuccessModalProps) => {
    // SuccessModal.tsx
    const surveyUrl =
        `${window.location.origin}/public/${surveyResponse.SurveyId}` +
        `?pinCode=${surveyResponse.PrivateKey!==''}`;

    const hasPrivateKey = surveyResponse.PrivateKey && surveyResponse.PrivateKey !== '';

    return (
          <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4">
                <div className="bg-main text-white p-6 max-w-md w-full mx-auto text-center rounded-lg shadow-xl">
                <h2 className="text-2xl font-semibold mb-4">Survey Published!</h2>
                <p className="mb-6">Your survey was published. You can now share the survey link.</p>

                <CopyBox label="Shareable Link:" value={surveyUrl} />

                {surveyResponse.PrivateKey && (
                    <CopyBox label="Private Key:" value={surveyResponse.PrivateKey} />
                )}

                <div className="mt-4">
                    <Button text="Go to Surveys" type="primary" onClick={onClose} />
                </div>
            </div>
        </div>
    );
};

export default SuccessModal;
