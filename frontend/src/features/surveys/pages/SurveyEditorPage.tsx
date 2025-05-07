import { useForm, useFieldArray, Controller } from 'react-hook-form';
import { useState } from 'react';
import { DndContext, closestCenter } from '@dnd-kit/core';
import {
    SortableContext,
    verticalListSortingStrategy,
} from '@dnd-kit/sortable';
import {Trash2, ChevronDown, ChevronUp, SaveIcon, PlusIcon} from 'lucide-react';
import { ArrowLeft, UploadIcon } from 'lucide-react';
import { Button } from '../components/Buttons/Button'
import {SortableItem} from "../components/SortableItem/SortableItem";
import {DesignedSurveyDto, MultipleChoiceDto, QuestionnaireDto} from "../../../shared/dto/DesignedSurveyDto";
import {handleSaveSurvey} from "../api/surveyApi";
import {useSelector} from "react-redux";
import {selectToken, selectUser} from "../../auth/slices/authSlice";
import {store} from "../../../app/store";

interface SurveyForm {
    title: string;
    questions: QuestionnaireDto[];
}

const SurveyEditorPage = () => {
    const { control, register, handleSubmit, watch, setValue } = useForm<SurveyForm>({
        defaultValues: {
            title: '',
            questions: [],
        },
    });

    const { fields, append, remove, move } = useFieldArray({
        control,
        name: 'questions',
    });

    const [openStates, setOpenStates] = useState<boolean[]>([]);

    const onDragEnd = (event: any) => {
        const { active, over } = event;
        if (active.id !== over?.id) {
            const oldIndex = fields.findIndex((q) => q.id === active.id);
            const newIndex = fields.findIndex((q) => q.id === over?.id);
            move(oldIndex, newIndex);
            const newOpenStates = [...openStates];
            const moved = newOpenStates.splice(oldIndex, 1)[0];
            newOpenStates.splice(newIndex, 0, moved);
            setOpenStates(newOpenStates);
        }
    };

    const handleAddQuestion = () => {
        append({
            SurveyId: 1,
            MultipleChoices: [],
            QuestionnaireId: Date.now(),
            QuestionnaireTitle: '',
            InputType: 'text',
            Range: '',
        });
        setOpenStates((prev) => [...prev, true]);
    };

    const toggleOpen = (index: number) => {
        setOpenStates((prev) =>
            prev.map((open, i) => (i === index ? !open : open))
        );
    };

    const handleDelete = (index: number) => {
        remove(index);
        setOpenStates((prev) => prev.filter((_, i) => i !== index));
    };

    const onSubmit = async (data: SurveyForm) => {
        try {
            const user = selectUser(store.getState())

            // Map SurveyForm to your DTO format if needed
            const surveyDto: DesignedSurveyDto = {
                SurveyId: undefined,
                SurveyTitle: data.title,
                SurveyDescription: '',
                StartDate: new Date(),
                EndDate: new Date(),
                UserId: user.userId,
                SurveyTypeId: 1,
                PrivateKey: undefined,
                Questionnaires: data.questions
            };

            await handleSaveSurvey(surveyDto);
            alert('Survey saved!');
        } catch (error) {
            console.error('Failed to submit survey:', error);
        }
    };

    return (
        <div className="min-h-screen bg-main text-white p-6 font-josefin">

            <div className={'mb-6'}>
            <Button text="Go Back" icon={<ArrowLeft size={16} />} type="secondary" onClick={() => window.location.href = '/surveys'} />
            </div>

            <input
                {...register('title')}
                placeholder="Survey title"
                className="bg-transparent border border-white p-2 text-lg font-semibold w-full mb-6 outline-none"
            />

            <h2 className={'text-xl font-semibold mt-4'}>
                Questions
            </h2>

            <DndContext collisionDetection={closestCenter} onDragEnd={onDragEnd}>
                <SortableContext
                    items={fields.map((f) => f.id)}
                    strategy={verticalListSortingStrategy}
                >
                    {fields.map((field, index) => (
                        <SortableItem key={field.id} id={field.id}>
                            <div className="border border-white w-full p-3 flex flex-col gap-2">
                                <div className="flex justify-between items-center">
                                    <input
                                        {...register(`questions.${index}.QuestionnaireTitle`)}
                                        placeholder="Question title"
                                        className="bg-transparent text-white flex-1 outline-none"
                                    />
                                    <div className="flex items-center gap-2">
                                        <select
                                            {...register(`questions.${index}.InputType`)}
                                            className="bg-main p-1 appearance-none"
                                        >
                                            <option value="checkbox" className="bg-main text-white">
                                                Checkbox
                                            </option>
                                            <option value="text" className="bg-main text-white">
                                                Text
                                            </option>
                                        </select>
                                        <button onClick={() => toggleOpen(index)}>
                                            {openStates[index] ? (
                                                <ChevronUp size={16} />
                                            ) : (
                                                <ChevronDown size={16} />
                                            )}
                                        </button>
                                        <button onClick={() => handleDelete(index)}>
                                            <Trash2 size={16} />
                                        </button>
                                    </div>
                                </div>

                                {openStates[index] && watch(`questions.${index}.InputType`) === 'checkbox' && (
                                    <Controller
                                        control={control}
                                        name={`questions.${index}.MultipleChoices`}
                                        render={({ field }) => (
                                            <div className="flex flex-col gap-2 mt-2">
                                                {field.value?.map((opt: MultipleChoiceDto, optIdx: number) => (
                                                    <input
                                                        key={optIdx}
                                                        value={opt.MultipleChoiceName}
                                                        onChange={(e) => {
                                                            if (field.value === undefined) return;

                                                            const updated = [...(field.value)];
                                                            updated[optIdx] = {
                                                                ...updated[optIdx],
                                                                MultipleChoiceName: e.target.value,
                                                            };
                                                            field.onChange(updated);
                                                        }}
                                                        className="bg-transparent text-white border border-white p-2 outline-none"
                                                    />
                                                ))}
                                                <button
                                                    type="button"
                                                    className="text-sm underline"
                                                    onClick={() => {
                                                        if (field.value === undefined) return;

                                                        field.onChange([...field.value, ''])
                                                    }}
                                                >
                                                    + Add option
                                                </button>
                                            </div>
                                        )}
                                    />
                                )}

                                {openStates[index] && watch(`questions.${index}.InputType`) === 'text' && (
                                    <div className="mt-2">
                                        <p className="text-sm text-white/70">Text answer preview:</p>
                                        <textarea
                                            className="bg-transparent text-white border border-white p-2 w-full resize-none outline-none"
                                            placeholder="User will write here..."
                                        />
                                    </div>
                                )}
                            </div>
                        </SortableItem>
                    ))}
                </SortableContext>
            </DndContext>

            <div className={'bg-main relative w-80 flex flex-col gap-4 w-full mt-6'}>
                <Button text="Add Question" type="secondary" onClick={handleAddQuestion} icon={<PlusIcon size={16} />} />
            </div>

            <div className="mt-10 flex justify-between items-center">
                <Button text="Delete Survey" icon={<Trash2 size={16} />} type="delete" onClick={() => window.location.href = '/surveys'} />
                <div className="flex gap-2">
                    <Button text={"Save Survey"} type={'primary'} icon={<SaveIcon size={16} />} onClick={handleSubmit(onSubmit)} />

                    <Button text={'Export Survey'} type={'secondary'} icon={<UploadIcon size={16} />} onClick={() => window.location.href = '/surveys'}/>
                </div>
            </div>
        </div>
    );
};

export default SurveyEditorPage;
