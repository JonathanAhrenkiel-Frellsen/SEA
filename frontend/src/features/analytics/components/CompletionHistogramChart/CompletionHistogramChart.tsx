import { useEffect, useMemo, useState } from "react";
import { BarChart, Bar, XAxis, YAxis, Tooltip, CartesianGrid, ResponsiveContainer } from "recharts";
import {SurveyCompletionRateDto} from "../../../../shared/dto/SurveyCompletionRateDto";
import {fetchSurveyCompletionRate} from "../../api/analyticsApi";

const CompletionHistogramChart = ({ surveyId }: { surveyId: string }) => {
    const [data, setData] = useState<SurveyCompletionRateDto | null>(null);

    useEffect(() => {
        fetchSurveyCompletionRate(surveyId).then(setData);
    }, [surveyId]);

    const histogram = useMemo(() => {
        const hist = data?.Histogram || [];
        if (hist.length <= 1) {
            return [...hist, { AnsweredCount: data?.TotalQuestions ?? 0, UserCount: 0 }];
        }
        return hist;
    }, [data]);

    return (
        <div className="w-full h-72 bg-secondary p-4 rounded">
            <h2 className="text-lg font-semibold mb-2">Survey Completion Histogram</h2>
            <ResponsiveContainer width="100%" height="100%">
                <BarChart data={histogram}>
                    <CartesianGrid strokeDasharray="3 3" />
                    <XAxis dataKey="AnsweredCount" stroke="#fff" label={{ value: "Questions Answered", position: "insideBottom", dy: 10 }} />
                    <YAxis stroke="#fff" />
                    <Tooltip />
                    <Bar dataKey="UserCount" fill="#82ca9d" />
                </BarChart>
            </ResponsiveContainer>
            <p className="mt-2 text-sm text-gray-300">Total questions: {data?.TotalQuestions}</p>
        </div>
    );
};

export default CompletionHistogramChart;
