import {useEffect, useState } from "react";
import { LineChart, Line, XAxis, YAxis, CartesianGrid, Tooltip, ResponsiveContainer } from "recharts";
import { format } from "date-fns";
import {SurveyResponseOverTimeDto} from "../../../../shared/dto/SurveyResponseOverTimeDto";
import {fetchSurveyResponsesOverTime} from "../../api/analyticsApi";

const CompletionsOverTimeChart = ({ surveyId }: { surveyId: string }) => {
    const [data, setData] = useState<SurveyResponseOverTimeDto[]>([]);

    useEffect(() => {
        fetchSurveyResponsesOverTime(surveyId).then(setData);
    }, [surveyId]);

    return (
        <div className="w-full h-72 bg-secondary p-4 rounded">
            <h2 className="text-lg font-semibold mb-2">Survey Completions Over Time</h2>
            <ResponsiveContainer width="100%" height="100%">
                <LineChart data={data}>
                    <CartesianGrid stroke="#444" strokeDasharray="5 5" />
                    <XAxis dataKey="Date" stroke="#fff" tickFormatter={(date) => format(new Date(date), "dd/MM/yyyy")} />
                    <YAxis stroke="#fff" />
                    <Tooltip labelFormatter={(date) => new Date(date).toLocaleDateString()} />
                    <Line type="monotone" dataKey="Count" stroke="#8884d8" strokeWidth={2} />
                </LineChart>
            </ResponsiveContainer>
        </div>
    );
};

export default CompletionsOverTimeChart;
