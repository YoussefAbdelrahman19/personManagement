'use client';

import { useState, useEffect } from 'react';
import { useRouter } from 'next/navigation';
import Link from 'next/link';
import personService from '@/services/personService';
import { UpdatePersonDto } from '@/types/person';
import { toast } from 'react-toastify';

interface FormErrors {
  firstName?: string;
  lastName?: string;
  age?: string;
}

export default function EditPersonPage({ params }: { params: { id: string } }) {
  const router = useRouter();
  const personId = parseInt(params.id);
  const [loading, setLoading] = useState(false);
  const [loadingPerson, setLoadingPerson] = useState(true);
  const [errors, setErrors] = useState<FormErrors>({});
  const [formData, setFormData] = useState<UpdatePersonDto>({
    firstName: '',
    lastName: '',
    age: 0,
  });

  useEffect(() => {
    loadPerson();
  }, [personId]);

  const loadPerson = async () => {
    try {
      setLoadingPerson(true);
      const person = await personService.getPersonById(personId);
      setFormData({
        firstName: person.firstName,
        lastName: person.lastName,
        age: person.age,
      });
    } catch (error) {
      console.error('Error loading person:', error);
      alert('Failed to load person data');
      router.push('/persons');
    } finally {
      setLoadingPerson(false);
    }
  };

  const validateForm = (): boolean => {
    const newErrors: FormErrors = {};

    if (!formData.firstName || formData.firstName.length < 2) {
      newErrors.firstName = 'First name must be at least 2 characters';
    }

    if (!formData.lastName || formData.lastName.length < 2) {
      newErrors.lastName = 'Last name must be at least 2 characters';
    }

    if (!formData.age || formData.age < 1 || formData.age > 150) {
      newErrors.age = 'Age must be between 1 and 150';
    }

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    if (!validateForm()) {
      toast.error('Please fix the errors in the form.', { position: 'top-right' });
      return;
    }

    try {
      setLoading(true);
      await personService.updatePerson(personId, formData);
      toast.success('Person updated successfully!', { position: 'top-right' });
      setTimeout(() => {
        router.push('/persons');
      }, 1200);
    } catch (error: any) {
      console.error('Error updating person:', error);
      toast.error(error?.message || 'Failed to update person. Please try again.', { position: 'top-right' });
    } finally {
      setLoading(false);
    }
  };

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target;
    setFormData(prev => ({
      ...prev,
      [name]: name === 'age' ? parseInt(value) || 0 : value,
    }));
    // Clear error for this field when user starts typing
    if (errors[name as keyof FormErrors]) {
      setErrors(prev => ({
        ...prev,
        [name]: undefined,
      }));
    }
  };

  if (loadingPerson) {
    return (
      <div className="flex justify-center items-center min-h-screen">
        <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600"></div>
      </div>
    );
  }

  return (
    <div className="container mx-auto px-4 py-8 max-w-2xl">
      <div className="bg-white shadow-lg rounded-lg p-8">
        <h1 className="text-3xl font-bold text-gray-800 mb-8">Edit Person</h1>

        <form onSubmit={handleSubmit} className="space-y-6">
          <div>
            <label htmlFor="firstName" className="block text-sm font-medium text-gray-700 mb-2">
              First Name
            </label>
            <input
              type="text"
              id="firstName"
              name="firstName"
              value={formData.firstName}
              onChange={handleChange}
              className={`w-full px-3 py-2 border rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500 ${
                errors.firstName ? 'border-red-500' : 'border-gray-300'
              }`}
              placeholder="Enter first name"
            />
            {errors.firstName && (
              <p className="mt-1 text-sm text-red-600">{errors.firstName}</p>
            )}
          </div>

          <div>
            <label htmlFor="lastName" className="block text-sm font-medium text-gray-700 mb-2">
              Last Name
            </label>
            <input
              type="text"
              id="lastName"
              name="lastName"
              value={formData.lastName}
              onChange={handleChange}
              className={`w-full px-3 py-2 border rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500 ${
                errors.lastName ? 'border-red-500' : 'border-gray-300'
              }`}
              placeholder="Enter last name"
            />
            {errors.lastName && (
              <p className="mt-1 text-sm text-red-600">{errors.lastName}</p>
            )}
          </div>

          <div>
            <label htmlFor="age" className="block text-sm font-medium text-gray-700 mb-2">
              Age
            </label>
            <input
              type="number"
              id="age"
              name="age"
              value={formData.age || ''}
              onChange={handleChange}
              className={`w-full px-3 py-2 border rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500 ${
                errors.age ? 'border-red-500' : 'border-gray-300'
              }`}
              placeholder="Enter age"
              min="1"
              max="150"
            />
            {errors.age && (
              <p className="mt-1 text-sm text-red-600">{errors.age}</p>
            )}
          </div>

          <div className="flex gap-4 pt-4">
            <button
              type="submit"
              disabled={loading}
              className={`flex-1 py-2 px-4 rounded-md text-white font-medium transition duration-200 ${
                loading
                  ? 'bg-gray-400 cursor-not-allowed'
                  : 'bg-blue-600 hover:bg-blue-700'
              }`}
            >
              {loading ? 'Updating...' : 'Update Person'}
            </button>
            <Link
              href="/persons"
              className="flex-1 py-2 px-4 bg-gray-200 text-gray-800 rounded-md text-center font-medium hover:bg-gray-300 transition duration-200"
            >
              Cancel
            </Link>
          </div>
        </form>
      </div>
    </div>
  );
}