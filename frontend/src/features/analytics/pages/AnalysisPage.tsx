import {
    LineChart,
    Line,
    XAxis,
    YAxis,
    CartesianGrid,
    Tooltip,
    ResponsiveContainer,
    BarChart,
    Bar,
} from 'recharts';
import { ArrowLeft, UploadIcon } from 'lucide-react';
import { Button } from "../../../shared/components/Buttons/Button";
import { useEffect, useMemo, useState } from "react";
import {
    fetchSurveyAnswersByUser,
    fetchSurveyCompletionRate,
    fetchSurveyResponsesOverTime
} from "../api/analyticsApi";
import { useParams } from "react-router-dom";
import {SurveyResponseOverTimeDto} from "../../../shared/dto/SurveyResponseOverTimeDto";
import {SurveyCompletionRateDto} from "../../../shared/dto/SurveyCompletionRateDto";

const SurveyAnalysisPage = () => {
    const { id } = useParams<{ id?: string }>();

    const [overTimeData, setOverTimeData] = useState<SurveyResponseOverTimeDto[]>([]);
    const [completionRate, setCompletionRate] = useState<SurveyCompletionRateDto | null>(null);
    const [answersTable, setAnswersTable] = useState<any[]>([]);
    const histogramWithPadding = useMemo(() => {
        const data = completionRate?.Histogram || [];
        if (data.length <= 1) {
            return [...data, { AnsweredCount: completionRate?.TotalQuestions ?? 0, UserCount: 0 }];
        }
        return data;
    }, [completionRate]);

    useEffect(() => {
        if (!id) return;

        fetchSurveyResponsesOverTime(id).then(setOverTimeData);
        fetchSurveyCompletionRate(id).then(setCompletionRate);
        fetchSurveyAnswersByUser(id, 0).then(setAnswersTable);
    }, [id]);

    return (
        <div className="min-h-screen bg-main text-white p-6 font-josefin flex flex-col gap-10">
            <div className="mb-6">
                <Button
                    text="Go Back"
                    icon={<ArrowLeft size={16} />}
                    type="secondary"
                    onClick={() => window.location.href = '/surveys'}
                />
            </div>

            {/* Chart 1: Completions over time */}
            <div className="w-full h-72 bg-secondary p-4 rounded">
                <h2 className="text-lg font-semibold mb-2">Survey Completions Over Time</h2>
                <ResponsiveContainer width="100%" height="100%">
                    <LineChart data={overTimeData}>
                        <CartesianGrid stroke="#444" strokeDasharray="5 5" />
                        <XAxis
                            dataKey="date"
                            stroke="#fff"
                            tickFormatter={(date) => new Date(date).toLocaleDateString()}
                        />
                        <YAxis stroke="#fff" />
                        <Tooltip labelFormatter={(date) => new Date(date).toLocaleDateString()} />
                        <Line type="monotone" dataKey="count" stroke="#8884d8" strokeWidth={2} />
                    </LineChart>
                </ResponsiveContainer>
            </div>

            {/* Chart 2: Completion rate histogram */}
            <div className="w-full h-72 bg-secondary p-4 rounded">
                <h2 className="text-lg font-semibold mb-2">Survey Completion Histogram</h2>
                <ResponsiveContainer width="100%" height="100%">
                    <BarChart data={histogramWithPadding}>
                        <CartesianGrid strokeDasharray="3 3" />
                        <XAxis dataKey="answeredCount" stroke="#fff" label={{ value: 'Questions Answered', position: 'insideBottom', dy: 10 }} />
                        <YAxis stroke="#fff" />
                        <Tooltip />
                        <Bar dataKey="userCount" fill="#82ca9d" />
                    </BarChart>
                </ResponsiveContainer>
                <p className="mt-2 text-sm text-gray-300">Total questions: {completionRate?.TotalQuestions}</p>
            </div>

            {/* Table: Survey Answers by User */}
            <div className="bg-secondary p-4 rounded">
                <h2 className="text-lg font-semibold mb-2">Answers by User (Page 1)</h2>
                <div className="overflow-auto max-h-96">
                    <table className="min-w-full text-left text-sm text-white border border-white">
                        <thead className="bg-main text-white sticky top-0">
                        <tr>
                            {answersTable.length > 0 &&
                                Object.keys(answersTable[0])
                                    .sort((a, b) => (a === "UserId" ? -1 : b === "UserId" ? 1 : a.localeCompare(b)))
                                    .map((key) => (
                                        <th key={key} className="px-4 py-2 border-b border-white">
                                            {key}
                                        </th>
                                    ))}
                        </tr>
                        </thead>
                        <tbody>
                        {answersTable.map((row, idx) => {
                            const sortedKeys = Object.keys(row).sort((a, b) =>
                                a === "UserId" ? -1 : b === "UserId" ? 1 : a.localeCompare(b)
                            );

                            return (
                                <tr key={idx} className="hover:bg-white hover:text-black transition">
                                    {sortedKeys.map((key) => (
                                        <td key={key} className="px-4 py-2 border-b border-white">
                                            {row[key]}
                                        </td>
                                    ))}
                                </tr>
                            );
                        })}
                        </tbody>
                    </table>
                </div>
            </div>


            <div className="flex justify-end mt-6">
                <Button
                    text="Export Survey"
                    type="secondary"
                    icon={<UploadIcon size={16} />}
                    onClick={() => window.location.href = '/surveys'}
                />
            </div>
        </div>
    );
};

export default SurveyAnalysisPage;
