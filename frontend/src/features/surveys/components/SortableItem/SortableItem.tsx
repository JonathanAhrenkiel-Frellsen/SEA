import {useSortable} from "@dnd-kit/sortable";
import {CSS} from "@dnd-kit/utilities";
import {GripVertical} from "lucide-react";

export const SortableItem = ({ id, children, readOnly }: { id: string; children: React.ReactNode; readOnly?: boolean }) => {
    const { attributes, listeners, setNodeRef, transform, transition } = useSortable({ id });

    const style = {
        transform: CSS.Transform.toString(transform),
        transition,
    };

    return (
        <div ref={setNodeRef} style={style} {...attributes} className="flex items-start gap-2">
            {!readOnly && (
                <div {...listeners} className="cursor-grab mt-2">
                    <GripVertical />
                </div>
            )}
            <div className="flex-1">{children}</div>
        </div>
    );
};
