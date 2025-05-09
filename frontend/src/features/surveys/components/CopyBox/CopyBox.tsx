interface CopyBoxProps {
  label?: string;
  value: string;
}

const CopyBox = ({ label, value }: CopyBoxProps) => {
  return (
    <div className="flex items-center gap-2 bg-secondary px-3 py-2 mb-4 break-all rounded-md">
            <span className="text-sm flex-1 overflow-hidden text-ellipsis">
                {label ? `${label} ${value}` : value}
            </span>
      <button
        onClick={() => navigator.clipboard.writeText(value)}
        className="hover:underline text-sm font-bold"
      >
        Copy
      </button>
    </div>
  );
};

export default CopyBox;
