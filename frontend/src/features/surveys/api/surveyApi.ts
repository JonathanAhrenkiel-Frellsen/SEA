import { DesignedSurveyDto } from "../../../shared/dto/DesignedSurveyDto";
import axios from "axios";
import { selectToken } from "../../auth/slices/authSlice";
import { store } from "../../../app/store";
import { SurveySaveAnswerDto } from "../../../shared/dto/SurveySaveAnswerDto";
import { ExperimenteeAppDto } from "../../../shared/dto/ExperimenteeAppDto";
import { EXPERIMENTEE_API_URL, EXPERIMENTER_API_URL } from "../../../shared/apiEndpoints";

export const handleSaveSurvey = async (survey: DesignedSurveyDto): Promise<DesignedSurveyDto | undefined> => {
  try {
    const response = await axios.post(`${EXPERIMENTER_API_URL}/surveys`, survey);

    if (response.status === 200 || response.status === 201) {
      return response.data;
    } else {
      console.warn('Unexpected response:', response.status, response.data);
    }
  } catch (error) {
    if (axios.isAxiosError(error)) {
      console.error('Error saving survey:', error.response?.data || error.message);
    } else {
      console.error('Unexpected error:', error);
    }
  }
};

export const fetchSurvey = async (surveyId: string, pinCode: string | undefined): Promise<DesignedSurveyDto> => {
  const jwt_token = selectToken(store.getState());

  if (!jwt_token) {
    throw new Error("JWT token not available");
  }

  const response = await fetch(`${EXPERIMENTER_API_URL}/surveys/${surveyId}`, {
    method: 'GET',
    headers: {
      'Authorization': `Bearer ${jwt_token}`,
      'X-Survey-Pin': pinCode || '',
      'Content-Type': 'application/json'
    }
  });

  if (!response.ok) {
    const errorText = await response.text();
    throw new Error(`Failed to fetch survey: ${response.status} ${errorText}`);
  }

  const data = await response.json();
  return data as DesignedSurveyDto;
}

export const deleteSurvey = async (surveyId: string): Promise<void> => {
  const jwt_token = selectToken(store.getState());

  if (!jwt_token) {
    throw new Error("JWT token not available");
  }

  const response = await fetch(`${EXPERIMENTER_API_URL}/surveys/${surveyId}`, {
    method: 'DELETE',
    headers: {
      'Authorization': `Bearer ${jwt_token}`,
      'Content-Type': 'application/json'
    }
  });

  if (!response.ok) {
    const errorText = await response.text();
    throw new Error(`Failed to delete survey: ${response.status} ${errorText}`);
  }
}

export const fetchSurveys = async (): Promise<DesignedSurveyDto[]> => {
  const jwt_token = selectToken(store.getState());

  if (!jwt_token) {
    throw new Error("JWT token not available");
  }

  const response = await fetch(`${EXPERIMENTER_API_URL}/surveys`, {
    method: 'GET',
    headers: {
      'Authorization': `Bearer ${jwt_token}`,
      'Content-Type': 'application/json'
    }
  });

  if (!response.ok) {
    const errorText = await response.text();
    throw new Error(`Failed to fetch surveys: ${response.status} ${errorText}`);
  }

  const data = await response.json();
  return data as DesignedSurveyDto[];
};

export const saveSurveyAnswer = async (surveyAnswer: SurveySaveAnswerDto): Promise<void> => {
  const jwt_token = selectToken(store.getState());

  if (!jwt_token) {
    throw new Error("JWT token not available");
  }

  const response = await fetch(`${EXPERIMENTEE_API_URL}/SaveSurveyAnswer`, {
    method: 'POST',
    headers: {
      'Authorization': `Bearer ${jwt_token}`,
      'Content-Type': 'application/json',
    },
    body: JSON.stringify(surveyAnswer),
  });

  if (!response.ok) {
    const errorText = await response.text();
    throw new Error(`Failed to fetch surveys: ${response.status} ${errorText}`);
  }

  return;
}

export async function completeSurvey(surveyId: string) {
  const jwt_token = selectToken(store.getState());

  const response = await fetch(`${EXPERIMENTEE_API_URL}/CompleteSurvey/${surveyId}`, {
    method: 'GET',
    headers: {
      'Authorization': `Bearer ${jwt_token}`,
      'Content-Type': 'application/json',
    },
  });

  if (!response.ok) {
    const errorText = await response.text();
    throw new Error(`Failed to fetch surveys: ${response.status} ${errorText}`);
  }
}

export const loadSurveyAnswers = async (surveyId: string, pin: string | undefined): Promise<ExperimenteeAppDto> => {
  const jwt_token = selectToken(store.getState());

  if (!jwt_token) {
    throw new Error("JWT token not available");
  }

  const response = await fetch(`${EXPERIMENTEE_API_URL}/LoadSurvey/${surveyId}`, {
    method: 'GET',
    headers: {
      'Authorization': `Bearer ${jwt_token}`,
      'Content-Type': 'application/json',
      'X-Survey-Pin': pin || '',
    },
  });

  if (!response.ok) {
    const errorText = await response.text();
    throw new Error(`Failed to fetch surveys: ${response.status} ${errorText}`);
  }

  return response.json();
}
