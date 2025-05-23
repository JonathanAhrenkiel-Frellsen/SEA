import { Button } from '../../../../shared/components/Buttons/Button';
import { Trash2, SaveIcon, UploadIcon } from 'lucide-react';
import {SurveyForm} from "../../types/SurveyForm";
import {deleteSurvey, handleSaveSurvey, exportSurveyStructure} from "../../api/surveyApi";
import {useNavigate} from "react-router-dom";
import {selectUser} from "../../../auth/slices/authSlice";
import {store} from "../../../../app/store";
import {DesignedSurveyDto} from "../../../../shared/dto/DesignedSurveyDto";
import React, { useState } from 'react';
import {parseToDate} from "../../services/timeService";



interface SurveyFooterActionsProps {
    id?: string;
    handleSubmit: any;
    handleShowSuccessModal: (survey: DesignedSurveyDto) => void;
    published?: boolean;
    onPublish: () => void;
}


const SurveyFooterActions = ({
                                 id,
                                 handleSubmit,
                                 published,
                                 onPublish
                             }: SurveyFooterActionsProps) => {
    const navigate = useNavigate();

    const onDelete = async () => {
        await deleteSurvey(id!);

        navigate('/surveys')
    }

    const [justSaved, setJustSaved] = useState(false);

    const onSubmit = async (data: SurveyForm) => {
        try {
            const user = selectUser(store.getState());
            const surveyDto: DesignedSurveyDto = {
                SurveyId: id ? parseInt(id) : undefined,
                SurveyTitle: data.title,
                SurveyDescription: '',
                StartDate: parseToDate(data.startDate) ?? new Date(),
                EndDate: parseToDate(data.endDate) ?? new Date(),
                UserId: user!.UserId,
                SurveyTypeId: 1,
                PrivateKey: data.isPrivate
                    ? Math.floor(1000 + Math.random() * 9000).toString()
                    : '',
                Questionnaires: data.questions.map((q, idx) => ({
                    ...q,
                    QuestionnairePos: idx,
                })),
                ResponseCount: 0,
                Published: false,
                IsPaused: false
            };

            const survey: DesignedSurveyDto | undefined = await handleSaveSurvey(
                surveyDto
            );

            if (survey) {
                setJustSaved(true);
                setTimeout(() => setJustSaved(false), 2000);
                navigate(`/surveys/${survey.SurveyId}/edit`);
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
                    icon={<Trash2 size={16} className="text-red-400" />}
                    type="delete"
                    onClick={handleSubmit(onDelete)}
                />
            ) : (
                <div />
            )}

            <div className="flex gap-2">
                {!published && (
                    <>
                        <Button
                            text={ justSaved ? "Saved!" : "Save Survey" }
                            type="primary"
                            icon={<SaveIcon size={16} />}
                            onClick={handleSubmit(onSubmit)}
                        />
                        <Button
                            text="Publish"
                            type="primary"
                            onClick={onPublish}
                        />
                    </>
                )}
                <Button
                    text="Export Setup"
                    type="secondary"
                    icon={<UploadIcon size={16} />}
                    onClick={() => exportSurveyStructure(Number(id))}
                />

            </div>

        </div>
    );
};

export default SurveyFooterActions;
