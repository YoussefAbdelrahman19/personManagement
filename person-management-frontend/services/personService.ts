import { Person, CreatePersonDto, UpdatePersonDto } from '@/types/person';

const API_BASE_URL = process.env.NEXT_PUBLIC_API_URL || 'https://localhost:7001/api';

class PersonService {
  private async handleResponse<T>(response: Response): Promise<T> {
    if (!response.ok) {
      const error = await response.json();
      throw new Error(error.message || 'An error occurred');
    }
    return response.json();
  }

  async getAllPersons(): Promise<Person[]> {
    const response = await fetch(`${API_BASE_URL}/persons`, {
      method: 'GET',
      headers: {
        'Content-Type': 'application/json',
      },
    });
    return this.handleResponse<Person[]>(response);
  }

  async getPersonById(id: number): Promise<Person> {
    const response = await fetch(`${API_BASE_URL}/persons/${id}`, {
      method: 'GET',
      headers: {
        'Content-Type': 'application/json',
      },
    });
    return this.handleResponse<Person>(response);
  }

  async createPerson(person: CreatePersonDto): Promise<Person> {
    const response = await fetch(`${API_BASE_URL}/persons`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
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
      },
      body: JSON.stringify(person),
    });
    return this.handleResponse<Person>(response);
  }

  async deletePerson(id: number): Promise<void> {
    const response = await fetch(`${API_BASE_URL}/persons/${id}`, {
      method: 'DELETE',
      headers: {
        'Content-Type': 'application/json',
      },
    });
    if (!response.ok) {
      const error = await response.json();
      throw new Error(error.message || 'An error occurred');
    }
  }
}

export default new PersonService();