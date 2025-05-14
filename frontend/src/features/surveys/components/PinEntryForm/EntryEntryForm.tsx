import TextInput from "../../../../shared/components/Input/TextInput";
import {Button} from "../../../../shared/components/Buttons/Button";

interface PinEntryFormProps {
    pin: string;
    setPin: (pin: string) => void;
    pinError: string;
    onSubmit: () => void;
}

const PinEntryForm = ({ pin, setPin, pinError, onSubmit }: PinEntryFormProps) => {
    return (
        <div className="p-6 max-w-md mx-auto">
            <h1 className="text-xl font-bold mb-4">Enter PIN Code</h1>
            <TextInput value={pin} setValue={setPin} />
            {pinError && <p className="text-red-500 mb-2">{pinError}</p>}
            <Button text="Submit PIN" type="primary" onClick={onSubmit} />
        </div>
    );
};

export default PinEntryForm;
