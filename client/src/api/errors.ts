import { isAxiosError } from 'axios';

interface ApiErrorResponse {
  message?: string;
}

export const getApiErrorMessage = (error: unknown, fallback: string) => {
  if (isAxiosError<ApiErrorResponse>(error)) {
    return error.response?.data?.message ?? fallback;
  }

  return fallback;
};
