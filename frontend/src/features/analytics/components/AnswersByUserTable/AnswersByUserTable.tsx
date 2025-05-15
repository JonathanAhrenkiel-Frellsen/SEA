import { useEffect, useState } from "react";
import {fetchSurveyAnswersByUser} from "../../api/analyticsApi";
import {Button} from "../../../../shared/components/Buttons/Button";

const AnswersByUserTable = ({ surveyId }: { surveyId: string }) => {
    const [answersTable, setAnswersTable] = useState<any[]>([]);
    const [currentPage, setCurrentPage] = useState(0);

    useEffect(() => {
        fetchSurveyAnswersByUser(surveyId, currentPage).then(setAnswersTable);
    }, [surveyId, currentPage]);

    return (
        <div className="bg-secondary p-4 rounded">
            <h2 className="text-lg font-semibold mb-2">Answers by User (Page {currentPage + 1})</h2>
            <div className="overflow-auto max-h-96">
                <table className="min-w-full text-left text-sm text-white border border-white">
                    <thead className="bg-main text-white sticky top-0">
                    <tr>
                        {answersTable.length > 0 &&
                            Object.keys(answersTable[0])
                                .sort((a, b) => (a === "UserId" ? -1 : b === "UserId" ? 1 : a.localeCompare(b)))
                                .map((key) => (
                                    <th key={key} className="px-4 py-2 border-b border-white">{key}</th>
                                ))}
                    </tr>
                    </thead>
                    <tbody>
                    {answersTable.map((row, idx) => {
                        const keys = Object.keys(row).sort((a, b) => (a === "UserId" ? -1 : b === "UserId" ? 1 : a.localeCompare(b)));
                        return (
                            <tr key={idx} className="hover:bg-white hover:text-black transition">
                                {keys.map((key) => (
                                    <td key={key} className="px-4 py-2 border-b border-white">{row[key]}</td>
                                ))}
                            </tr>
                        );
                    })}
                    </tbody>
                </table>
            </div>
            <div className="flex justify-between mt-4">
                <Button text="Previous" type="secondary" onClick={() => setCurrentPage(p => Math.max(p - 1, 0))} disabled={currentPage === 0} />
                <span className="text-white font-semibold">Page {currentPage + 1}</span>
                <Button text="Next" type="secondary" onClick={() => setCurrentPage(p => p + 1)} />
            </div>
        </div>
    );
};

export default AnswersByUserTable;
