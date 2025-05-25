import React, { useEffect, useState } from 'react';
import { useParams, useNavigate, useLocation } from 'react-router-dom';
import { useForm, useFieldArray } from 'react-hook-form';
import { ArrowLeft, PlusIcon } from 'lucide-react';
import { Button } from '../../../shared/components/Buttons/Button';
import EditSurveyHeader from '../components/EditSurveyHeader/EditSurveyHeader';
import SurveyQuestionList from '../components/SurveyQuestionList/SurveyQuestionList';
import SurveyFooterActions from '../components/SurveyFooterActions/SurveyFooterActions';
import SuccessModal from '../components/Modals/SuccessModal/SuccessModal';
import { fetchSurvey, publishSurvey, pauseSurvey, resumeSurvey } from '../api/surveyApi';
import { DesignedSurveyDto, QuestionnaireDto } from '../../../shared/dto/DesignedSurveyDto';
import { SurveyForm } from '../types/SurveyForm';
import {StartEndInput} from "../components/StartEndDateInput/StartEndInput";

const SurveyEditorPage: React.FC = () => {
    const { control, register, handleSubmit, watch, setValue, reset, formState } = useForm<SurveyForm>({
        defaultValues: { title: '', isPrivate: false, questions: [] },
        mode: "onChange"
    });
    console.log("Form errors:", formState.errors);
    const [pinCode, setPinCode] = useState<string | undefined>(undefined);
    const { id } = useParams<{ id: string }>();
    const location = useLocation();
    const importedSurvey = location.state?.importedSurvey;
    const navigate = useNavigate();
    const { control, register, handleSubmit, watch, setValue } = useForm<SurveyForm>({
        defaultValues: { title: '', isPrivate: false, questions: [], endDate: null, startDate: null },
    });
    const { fields, append, remove, move } = useFieldArray({ control, name: 'questions' });

    // State for published and pause
    const [published, setPublished] = useState<boolean>(false);
    const [isPaused, setIsPaused] = useState<boolean>(false);
    const [openStates, setOpenStates] = useState<boolean[]>([]);
    const [showSuccessModal, setShowSuccessModal] = useState(false);
    const [surveyResponse, setSurveyResponse] = useState<DesignedSurveyDto | null>(null);
    const [copied, setCopied] = useState(false);
    const [copiedPin, setCopiedPin] = useState(false);
    const isPrivate = watch('isPrivate');
    const surveyLink = `${window.location.origin}/public/${id}?pinCode=${isPrivate ? 'true' : 'false'}`;

    const handleCopy = () => {
        navigator.clipboard.writeText(surveyLink);
        setCopied(true);
        setTimeout(() => setCopied(false), 2000);
    };

    const handleCopyPin = () => {
        if (!pinCode) return;
        navigator.clipboard.writeText(pinCode);
        setCopiedPin(true);
        setTimeout(() => setCopiedPin(false), 2000);
    };

    // Hent eksisterende survey hvis id findes
    useEffect(() => {
        if (!id) return;
        fetchSurvey(id!, undefined)
            .then((data: DesignedSurveyDto) => {
                setValue('title', data.SurveyTitle || '');
                setValue('isPrivate', data.PrivateKey !== '');
                setValue('questions', data.Questionnaires || []);
                setValue(
                  'startDate',
                  data.StartDate ? new Date(data.StartDate).toISOString().slice(0, 10) : ''
                );
                setValue(
                  'endDate',
                  data.EndDate ? new Date(data.EndDate).toISOString().slice(0, 10) : ''
                );
                setPublished(data.Published ?? false);
                setIsPaused(data.IsPaused ?? false);
                setOpenStates(new Array(data.Questionnaires?.length ?? 0).fill(false));
                setPinCode(data.PrivateKey || undefined);
            })
            .catch(console.error);
    }, [id, setValue]);

    // Importeret survey fra CSV
    useEffect(() => {
        if (!id && importedSurvey && importedSurvey.length > 0) {
            reset({
                title: importedSurvey[0]['Survey Title'] || '',
                isPrivate: (importedSurvey[0]['Accessibility'] || '').toLowerCase() === 'private',
                questions: importedSurvey.map((row: any, idx: number) => ({
                    SurveyId: 1,
                    QuestionnaireId: 0,
                    QuestionnairePos: idx,
                    QuestionnaireTitle: row['Question'] || '',
                    InputType: (row['Answer Type'] || 'text').toLowerCase(),
                    Range: '',
                    MultipleChoices: (row['Answer Options'] || '').split(';').map((s: string) => s.trim()).filter(Boolean),
                }))
            });
            setOpenStates(new Array(importedSurvey.length).fill(false));
        }
    }, [importedSurvey, reset, id]);

    const handleAddQuestion = () => {
        append({
            SurveyId: 1,
            QuestionnaireId: 0,
            QuestionnairePos: fields.length,
            QuestionnaireTitle: '',
            InputType: 'text',
            Range: '',
            MultipleChoices: [],
        } as QuestionnaireDto);
        setOpenStates(prev => [...prev, true]);
    };

    const handleShowSuccessModal = (survey: DesignedSurveyDto) => {
        setSurveyResponse(survey);
        setShowSuccessModal(true);
    };

    const handlePublish = async () => {
        if (!id) return;
        try {
            await publishSurvey(id);
            setPublished(true);
            const fresh = await fetchSurvey(id!, undefined);
            handleShowSuccessModal(fresh);
        } catch (err) {
            console.error("Publish failed:", err);
            alert("Could not publish. Check console.");
        }
    };

    return (
        <div className="min-h-screen bg-main text-white p-6 font-josefin">
            {/* Go Back */}
            <div className="mb-6">
                <Button
                    text="Go Back"
                    type="secondary"
                    icon={<ArrowLeft size={16} />}
                    onClick={() => navigate('/surveys')}
                />
            </div>

            {/* Pause / Resume panel */}
            {published && (
                <div className="my-4 flex items-center gap-4">
                    {isPaused
                        ? <Button
                            text="Resume Survey"
                            type="secondary"
                            onClick={() => resumeSurvey(Number(id)).then(() => setIsPaused(false))}
                        />
                        : <Button
                            text="Pause Survey"
                            type="secondary"
                            onClick={() => pauseSurvey(Number(id)).then(() => setIsPaused(true))}
                        />
                    }
                    <span>Status: {isPaused ? 'Paused' : 'Active'}</span>
                </div>
            )}

            {/* Locked message when published */}
            {published && (
                <div className="bg-green-700 text-white rounded-xl px-4 py-2 mb-4 font-semibold text-center">
                    This survey is published and cannot be edited.
                </div>
            )}

            {/* Survey Header & Questions */}
            <EditSurveyHeader
                register={register}
                watch={watch}
                setValue={setValue}
                readOnly={published}
                surveyLink={published ? surveyLink : undefined}
                onCopy={handleCopy}
                copied={copied}
                pinCode={pinCode}
                onCopyPin={handleCopyPin}
                copiedPin={copiedPin}
            />

            <StartEndInput control={control} />

            <h2 className="text-xl font-semibold mt-4">Questions</h2>
            <SurveyQuestionList
                fields={fields}
                control={control}
                register={register}
                watch={watch}
                openStates={openStates}
                setOpenStates={setOpenStates}
                move={move}
                remove={remove}
                readOnly={published}
            />

            {/* Add Question */}
            {!published && (
                <div className="mt-6">
                    <Button
                        text="Add Question"
                        type="secondary"
                        icon={<PlusIcon size={16} />}
                        onClick={handleAddQuestion}
                    />
                </div>
            )}

            {/* Save & Publish actions */}
            <SurveyFooterActions
                {...(id ? { id: String(id) } : {})}
                handleSubmit={handleSubmit}
                handleShowSuccessModal={handleShowSuccessModal}
                published={published}
                onPublish={handlePublish}
            />

            {/* Success Modal */}
            {showSuccessModal && surveyResponse && (
                <SuccessModal
                    surveyResponse={surveyResponse}
                    onClose={() => navigate('/surveys')}
                />
            )}
        </div>
    );
};

export default SurveyEditorPage;
