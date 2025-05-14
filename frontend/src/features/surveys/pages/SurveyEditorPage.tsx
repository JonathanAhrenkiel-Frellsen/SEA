import { useForm, useFieldArray, Controller } from 'react-hook-form';
import React, {useEffect, useState} from 'react';
import {PlusIcon} from 'lucide-react';
import { ArrowLeft } from 'lucide-react';
import { Button } from '../../../shared/components/Buttons/Button'
import {DesignedSurveyDto} from "../../../shared/dto/DesignedSurveyDto";
import {fetchSurvey} from "../api/surveyApi";
import {useNavigate, useParams} from "react-router-dom";
import EditSurveyHeader from '../components/EditSurveyHeader/EditSurveyHeader';
import SurveyFooterActions from '../components/SurveyFooterActions/SurveyFooterActions';
import {SurveyForm} from "../types/SurveyForm";
import SuccessModal from "../components/Modals/SuccessModal/SuccessModal";
import SurveyQuestionList from "../components/SurveyQuestionList/SurveyQuestionList";

const SurveyEditorPage = () => {
    const { id } = useParams<{ id?: string }>();
    const navigate = useNavigate();
    const [showSuccessModal, setShowSuccessModal] = useState(false);
    const [surveyResponse, setSurveyResponse] = useState<DesignedSurveyDto | null>(null);

    const { control, register, handleSubmit, watch, setValue } = useForm<SurveyForm>({
        defaultValues: {
            title: '',
            isPrivate: false,
            questions: [],
        },
    });

    const { fields, append, remove, move } = useFieldArray({
        control,
        name: 'questions',
    });

    const [openStates, setOpenStates] = useState<boolean[]>([]);

    useEffect(() => {
        if (!id) return;

        const survey = fetchSurvey(id, undefined);

        survey.then((data) => {
            const questions = data.Questionnaires!.map((question) => ({
                ...question,
                QuestionnaireId: question.QuestionnaireId,
                QuestionnairePos: question.QuestionnairePos,
                MultipleChoices: question.MultipleChoices.map((choice) => ({
                    ...choice,
                    MultipleChoiceId: choice.MultipleChoiceId,
                })),
            }));

            setOpenStates(new Array(questions.length).fill(false));
            setValue('questions', questions);
            setValue('isPrivate', data.PrivateKey !== '');
            setValue('title', data.SurveyTitle || '');
        }).catch((error) => {
            console.error('Error fetching survey:', error);
        });
    }, [id]);

    const handleAddQuestion = () => {
        append({
            SurveyId: 1,
            MultipleChoices: [],
            QuestionnaireId: 0,
            QuestionnaireTitle: '',
            InputType: 'text',
            Range: '',
            QuestionnairePos: fields.length
        });
        setOpenStates((prev) => [...prev, true]);
    };

    const handleShowSuccessModal = async (survey: DesignedSurveyDto) => {
        setShowSuccessModal(true);
        setSurveyResponse(survey);
    }

    return (
        <div className="min-h-screen bg-main text-white p-6 font-josefin">
            <div className="mb-6">
                <Button text="Go Back" icon={<ArrowLeft size={16} />} type="secondary" onClick={() => navigate('/surveys')} />
            </div>

            <EditSurveyHeader register={register} watch={watch} setValue={setValue} />

            <h2 className="text-xl font-semibold mt-4">Questions</h2>
            <SurveyQuestionList
                fields={fields}
                setOpenStates={setOpenStates}
                control={control}
                register={register}
                watch={watch}
                openStates={openStates}
                move={move}
                remove={remove}
                />

            <div className="bg-main relative flex flex-col gap-4 w-full mt-6">
                <Button text="Add Question" type="secondary" onClick={handleAddQuestion} icon={<PlusIcon size={16} />} />
            </div>

            <SurveyFooterActions
                id={id}
                handleSubmit={handleSubmit}
                handleShowSuccessModal={handleShowSuccessModal}
            />

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
