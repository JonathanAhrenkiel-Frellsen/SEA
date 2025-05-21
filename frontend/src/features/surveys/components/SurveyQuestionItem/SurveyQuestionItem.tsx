import { Controller, UseFormRegister, UseFormWatch, Control } from 'react-hook-form';
import { ChevronDown, ChevronUp, Trash2 } from 'lucide-react';
import { MultipleChoiceDto } from '../../../../shared/dto/DesignedSurveyDto';
import {UniqueIdentifier} from "@dnd-kit/core";

interface SurveyQuestionItemProps {
    index: number;
    field: { id: UniqueIdentifier };
    open: boolean;
    toggleOpen: (i: number) => void;
    handleDelete: (i: number) => void;
    register: UseFormRegister<any>;
    control: Control<any>;
    watch: UseFormWatch<any>;
    readOnly?: boolean;
}

const SurveyQuestionItem = ({
                                index,
                                field,
                                open,
                                toggleOpen,
                                handleDelete,
                                register,
                                control,
                                watch,
                                readOnly = false
                            }: SurveyQuestionItemProps) => {
    const inputType = watch(`questions.${index}.InputType`);

    return (
        <div className="border border-white w-full p-3 flex flex-col gap-2">
            <div className="flex justify-between items-center">
                <input
                    {...register(`questions.${index}.QuestionnaireTitle`)}
                    placeholder="Question title"
                    className="bg-transparent text-white flex-1 outline-none"
                    disabled={readOnly}
                />
                <div className="flex items-center gap-2">
                    <select
                        {...register(`questions.${index}.InputType`)}
                        className="bg-main p-1 appearance-none"
                        disabled={readOnly}
                    >
                        <option value="checkbox" className="bg-main text-white">Checkbox</option>
                        <option value="text" className="bg-main text-white">Text</option>
                    </select>
                    {!readOnly && (
                        <>
                            <button type="button" onClick={() => toggleOpen(index)}>
                                {open ? <ChevronUp size={16} /> : <ChevronDown size={16} />}
                            </button>
                            <button type="button" className='ml-1 p-1 rounded hover:bg-red-700 transition' onClick={() => handleDelete(index)}>
                                <Trash2 size={20} className="text-red-400" />
                            </button>
                        </>
                    )}
                </div>
            </div>

            {open && inputType === 'checkbox' && (
                <Controller
                    control={control}
                    name={`questions.${index}.MultipleChoices`}
                    render={({ field }) => (
                        <div className="flex flex-col gap-2 mt-2">
                            {field.value?.map((opt: MultipleChoiceDto, optIdx: number) => (
                                <div key={optIdx} className="flex items-center gap-2">
                                    <input
                                        value={opt.MultipleChoiceName}
                                        onChange={(e) => {
                                            if (!field.value) return;
                                            const updated = [...field.value];
                                            updated[optIdx] = {
                                                ...updated[optIdx],
                                                MultipleChoiceName: e.target.value,
                                            };
                                            field.onChange(updated);
                                        }}
                                        className="bg-transparent text-white border border-white p-2 outline-none flex-1"
                                        disabled={readOnly}
                                    />
                                    {!readOnly && (
                                        <button
                                            type="button"
                                            className="ml-1 p-1 rounded hover:bg-red-700 transition"
                                            onClick={() => {
                                                if (!field.value) return;
                                                const updated = field.value.filter((_: MultipleChoiceDto, idx: number) => idx !== optIdx);
                                                field.onChange(updated);
                                            }}
                                            aria-label="Delete option"
                                        >
                                            <Trash2 size={16} className="text-red-400" />
                                        </button>
                                    )}
                                </div>
                            ))}
                            {!readOnly && (
                                <button
                                    type="button"
                                    className="text-sm underline"
                                    onClick={() => {
                                        if (!field.value) return;
                                        field.onChange([
                                            ...field.value,
                                            { MultipleChoiceId: 0, MultipleChoiceName: '' },
                                        ]);
                                    }}
                                >
                                    + Add option
                                </button>
                            )}
                        </div>
                    )}
                />
            )}

            {open && inputType === 'text' && (
                <div className="mt-2">
                    <p className="text-sm text-white/70">Text answer preview:</p>
                    <textarea
                        className="bg-transparent text-white border border-white p-2 w-full resize-none outline-none"
                        placeholder="User will write here..."
                        disabled
                    />
                </div>
            )}
        </div>
    );
};

export default SurveyQuestionItem;
