import {DesignedSurveyDto} from "../../../shared/dto/DesignedSurveyDto";
import axios from "axios";
import {selectToken} from "../../auth/slices/authSlice";
import {store} from "../../../app/store";

const AUTH_API_URL = `${process.env.REACT_APP_BASE_URL}/api/ExperimenterApp`;

export const handleSaveSurvey = async (survey: DesignedSurveyDto): Promise<void> => {
  try {
    const response = await axios.post(`${AUTH_API_URL}/surveys`, survey);

    if (response.status === 200 || response.status === 201) {
      console.log('Survey saved successfully:', response.data);
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

export const fetchSurvey = async (surveyId: string): Promise<DesignedSurveyDto> => {
  const jwt_token = selectToken(store.getState());

  if (!jwt_token) {
    throw new Error("JWT token not available");
  }

  const response = await fetch(`${AUTH_API_URL}/surveys/${surveyId}`, {
    method: 'GET',
    headers: {
      'Authorization': `Bearer ${jwt_token}`,
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

  const response = await fetch(`${AUTH_API_URL}/surveys/${surveyId}`, {
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

  const response = await fetch(`${AUTH_API_URL}/surveys`, {
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
