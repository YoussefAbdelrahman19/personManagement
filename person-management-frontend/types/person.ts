export interface Person {
  personId: number;
  firstName: string;
  lastName: string;
  age: number;
  createdAt: string;
  updatedAt?: string;
}

export interface CreatePersonDto {
  firstName: string;
  lastName: string;
  age: number;
}

export interface UpdatePersonDto {
  firstName: string;
  lastName: string;
  age: number;
}

export interface ApiError {
  message: string;
  errors?: Record<string, string[]>;
}