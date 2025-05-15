import { Button } from '../../../../shared/components/Buttons/Button';
import { Trash2, SaveIcon, UploadIcon } from 'lucide-react';
import {SurveyForm} from "../../types/SurveyForm";
import {deleteSurvey, handleSaveSurvey} from "../../api/surveyApi";
import {useNavigate} from "react-router-dom";
import {selectUser} from "../../../auth/slices/authSlice";
import {store} from "../../../../app/store";
import {DesignedSurveyDto} from "../../../../shared/dto/DesignedSurveyDto";

interface SurveyFooterActionsProps {
    id?: string;
    handleSubmit: any;
    handleShowSuccessModal: (survey: DesignedSurveyDto) => void;
}

const SurveyFooterActions = ({
                                 id,
                                 handleSubmit,
                                 handleShowSuccessModal
                             }: SurveyFooterActionsProps) => {
    const navigate = useNavigate();

    const onDelete = async () => {
        await deleteSurvey(id!);

        navigate('/surveys')
    }

    const onSubmit = async (data: SurveyForm) => {
        try {
            const user = selectUser(store.getState());
            const surveyDto: DesignedSurveyDto = {
                SurveyId: id ? parseInt(id) : undefined,
                SurveyTitle: data.title,
                SurveyDescription: '',
                StartDate: new Date(),
                EndDate: new Date(),
                UserId: user!.UserId,
                SurveyTypeId: 1,
                PrivateKey: data.isPrivate
                    ? Math.floor(1000 + Math.random() * 9000).toString()
                    : '',
                Questionnaires: data.questions.map((q, idx) => ({
                    ...q,
                    QuestionnairePos: idx,
                })),
                ResponseCount : 0,
            };

            const survey: DesignedSurveyDto | undefined = await handleSaveSurvey(
                surveyDto
            );

            if (survey) {
                handleShowSuccessModal(survey);
            }
        } catch (error) {
            console.error('Failed to submit survey:', error);
        }
    };

    return (
        <div className="mt-10 flex justify-between items-center">
            {id ? (
                <Button
                    text="Delete Survey"
                    icon={<Trash2 size={16} />}
                    type="delete"
                    onClick={handleSubmit(onDelete)}
                />
            ) : (
                <div />
            )}

            <div className="flex gap-2">
                <Button
                    text="Save Survey"
                    type="primary"
                    icon={<SaveIcon size={16} />}
                    onClick={handleSubmit(onSubmit)}
                />
                <Button
                    text="Export Survey"
                    type="secondary"
                    icon={<UploadIcon size={16} />}
                    onClick={() => (window.location.href = '/surveys')}
                />
            </div>
        </div>
    );
};

export default SurveyFooterActions;
