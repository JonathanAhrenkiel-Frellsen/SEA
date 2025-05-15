import { DndContext, closestCenter } from '@dnd-kit/core';
import { SortableContext, verticalListSortingStrategy } from '@dnd-kit/sortable';
import { UniqueIdentifier } from '@dnd-kit/core';
import {Control, UseFormRegister, UseFormWatch, FieldValues, UseFieldArrayRemove} from 'react-hook-form';

import { SortableItem } from '../SortableItem/SortableItem';
import SurveyQuestionItem from '../SurveyQuestionItem/SurveyQuestionItem';
import {SurveyForm} from "../../types/SurveyForm";

interface SurveyQuestionListProps {
    fields: { id: string }[];
    control: Control<SurveyForm>;
    register: UseFormRegister<SurveyForm>;
    watch: UseFormWatch<SurveyForm>;
    move: (from: number, to: number) => void;
    setOpenStates: any;
    openStates: boolean[];
    remove: UseFieldArrayRemove;
    readOnly?: boolean;

}

const SurveyQuestionList = ({
                                fields,
                                control,
                                register,
                                watch,
                                openStates,
                                move,
                                setOpenStates,
                                remove,
                                readOnly
                            }: SurveyQuestionListProps) => {
    const onDragEnd = (event: any) => {
        const { active, over } = event;
        if (active.id !== over?.id) {
            const oldIndex = fields.findIndex((q) => q.id === active.id);
            const newIndex = fields.findIndex((q) => q.id === over?.id);
            move(oldIndex, newIndex);
        }
    };

    const toggleOpen = (index: number) => {
        setOpenStates((prev: boolean[]) =>
            prev.map((open: boolean, i: number) => (i === index ? !open : open))
        );
    };

    const handleDelete = (index: number) => {
        remove(index);
        setOpenStates((prev: boolean[]) => prev.filter((_, i) => i !== index));
    };

    return (
        <DndContext collisionDetection={closestCenter} onDragEnd={onDragEnd}>
            <SortableContext items={fields.map((f) => f.id)} strategy={verticalListSortingStrategy}>
                {fields.map((field, index) => (
                    <SortableItem key={field.id} id={field.id.toString()}>
                        <SurveyQuestionItem
                            index={index}
                            field={field}
                            open={openStates[index]}
                            toggleOpen={toggleOpen}
                            handleDelete={handleDelete}
                            register={register}
                            control={control}
                            watch={watch}
                            readOnly={readOnly}
                        />
                    </SortableItem>
                ))}
            </SortableContext>
        </DndContext>
    );
};

export default SurveyQuestionList;
