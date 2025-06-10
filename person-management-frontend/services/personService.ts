import { Person, CreatePersonDto, UpdatePersonDto } from '@/types/person';

const API_BASE_URL = 'http://localhost:5296/api';

class ApiError extends Error {
  constructor(message: string, public status?: number) {
    super(message);
    this.name = 'ApiError';
  }
}

class PersonService {
  private async handleResponse<T>(response: Response): Promise<T> {
    if (!response.ok) {
      const errorData = await response.json().catch(() => ({}));
      throw new ApiError(errorData.message || 'An error occurred', response.status);
    }
    return response.json();
  }

  async getAllPersons(): Promise<Person[]> {
    const response = await fetch(`${API_BASE_URL}/persons`, {
      headers: {
        'Cache-Control': 'no-cache',
      },
    });
    return this.handleResponse<Person[]>(response);
  }

  async getPersonById(id: number): Promise<Person> {
    const response = await fetch(`${API_BASE_URL}/persons/${id}`, {
      headers: {
        'Cache-Control': 'no-cache',
      },
    });
    return this.handleResponse<Person>(response);
  }

  async createPerson(person: CreatePersonDto): Promise<Person> {
    const response = await fetch(`${API_BASE_URL}/persons`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        'Cache-Control': 'no-cache',
      },
      body: JSON.stringify(person),
    });
    return this.handleResponse<Person>(response);
  }

  async updatePerson(id: number, person: UpdatePersonDto): Promise<Person> {
    const response = await fetch(`${API_BASE_URL}/persons/${id}`, {
      method: 'PUT',
      headers: {
        'Content-Type': 'application/json',
        'Cache-Control': 'no-cache',
      },
      body: JSON.stringify(person),
    });
    return this.handleResponse<Person>(response);
  }

  async deletePerson(id: number): Promise<void> {
    const response = await fetch(`${API_BASE_URL}/persons/${id}`, {
      method: 'DELETE',
      headers: {
        'Cache-Control': 'no-cache',
      },
    });
    if (!response.ok) {
      const errorData = await response.json().catch(() => ({}));
      throw new ApiError(errorData.message || 'Failed to delete person', response.status);
    }
  }
}

export default new PersonService();