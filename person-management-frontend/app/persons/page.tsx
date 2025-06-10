'use client';

import { useEffect, useState } from 'react';
import Link from 'next/link';
import { useRouter } from 'next/navigation';
import personService from '@/services/personService';
import { Person, UpdatePersonDto } from '@/types/person';
import Swal from 'sweetalert2';
import { toast } from 'react-toastify';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';

export default function PersonsPage() {
  const queryClient = useQueryClient();
  const [loadingTimeout, setLoadingTimeout] = useState(false);

  const { data: persons = [], isLoading, error } = useQuery<Person[], Error>({
    queryKey: ['persons'],
    queryFn: () => personService.getAllPersons(),
    refetchOnWindowFocus: true,
    refetchOnMount: true,
    staleTime: 0,
    gcTime: 0,
  });

  useEffect(() => {
    let timeout: NodeJS.Timeout;
    if (isLoading) {
      timeout = setTimeout(() => setLoadingTimeout(true), 5000);
    } else {
      setLoadingTimeout(false);
    }
    return () => clearTimeout(timeout);
  }, [isLoading]);

  const deleteMutation = useMutation<void, Error, number, { previousPersons: Person[] | undefined }>({
    mutationFn: (id: number) => personService.deletePerson(id),
    onMutate: async (id) => {
      // Cancel any outgoing refetches so they don't overwrite our optimistic update
      await queryClient.cancelQueries({ queryKey: ['persons'] });
      // Snapshot the previous value
      const previousPersons = queryClient.getQueryData<Person[]>(['persons']);
      // Optimistically update to the new value
      queryClient.setQueryData<Person[]>(['persons'], (old) => old?.filter(p => p.personId !== id) || []);
      // Return a context object with the snapshotted value
      return { previousPersons };
    },
    onError: (err, id, context) => {
      // If the mutation fails, use the context returned from onMutate to roll back
      queryClient.setQueryData(['persons'], context?.previousPersons);
      toast.error(err?.message || 'Failed to delete person', { position: 'top-right', autoClose: 2000 });
    },
    onSuccess: () => {
      // Force a refetch to ensure the UI shows the most recent data
      queryClient.invalidateQueries({ queryKey: ['persons'] });
      toast.success('Person deleted successfully!', { position: 'top-right', autoClose: 2000 });
    },
  });

  const handleDelete = async (id: number) => {
    const result = await Swal.fire({
      title: 'Are you sure?',
      text: 'This action cannot be undone!',
      icon: 'warning',
      showCancelButton: true,
      confirmButtonColor: '#d33',
      cancelButtonColor: '#3085d6',
      confirmButtonText: 'Yes, delete it!',
    });
    if (result.isConfirmed) {
      deleteMutation.mutate(id);
    }
  };

  const router = useRouter();
  const handleEdit = (id: number) => {
    router.push(`/persons/edit/${id}`);
  };

  const updateMutation = useMutation<Person, Error, { id: number; data: UpdatePersonDto }, { previousPerson: Person | undefined }>({
    mutationFn: ({ id, data }) => personService.updatePerson(id, data),
    onMutate: async ({ id, data }) => {
      await queryClient.cancelQueries({ queryKey: ['persons'] });
      const previousPerson = queryClient.getQueryData<Person>(['persons', id]);
      queryClient.setQueryData<Person>(['persons', id], (old) => old ? { ...old, ...data } as Person : undefined);
      return { previousPerson };
    },
    onError: (err, variables, context) => {
      queryClient.setQueryData(['persons', variables.id], context?.previousPerson);
      toast.error(err?.message || 'Failed to update person', { position: 'top-right', autoClose: 2000 });
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['persons'] });
      toast.success('Person updated successfully!', { position: 'top-right', autoClose: 2000 });
    },
  });

  if (isLoading) {
    return (
      <div className="flex flex-col justify-center items-center min-h-screen">
        <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600 mb-4"></div>
        <span className="text-gray-600">Loading persons...</span>
        {loadingTimeout && (
          <div className="mt-4 text-red-600 font-semibold">
            Loading is taking longer than expected. Please check your connection or try reloading the page.
          </div>
        )}
      </div>
    );
  }

  if (error) {
    return (
      <div className="container mx-auto px-4 py-8">
        <div className="bg-red-100 border border-red-400 text-red-700 px-4 py-3 rounded mb-4">
          <p>{error instanceof Error ? error.message : 'Failed to load persons'}</p>
          <button
            onClick={() => queryClient.invalidateQueries({ queryKey: ['persons'] })}
            className="mt-2 bg-red-600 text-white px-4 py-2 rounded hover:bg-red-700"
          >
            Retry
          </button>
        </div>
      </div>
    );
  }

  if (!Array.isArray(persons)) {
    return (
      <div className="container mx-auto px-4 py-8">
        <div className="bg-red-100 border border-red-400 text-red-700 px-4 py-3 rounded mb-4">
          <p>Unexpected error: Data format is invalid.</p>
        </div>
      </div>
    );
  }

  return (
    <div className="container mx-auto px-4 py-8">
      <div className="flex justify-between items-center mb-8">
        <h1 className="text-3xl font-bold text-gray-800">Persons Management</h1>
        <Link
          href="/persons/create"
          className="bg-blue-600 text-white px-6 py-2 rounded-lg hover:bg-blue-700 transition duration-200"
        >
          Add New Person
        </Link>
      </div>

      {persons.length === 0 ? (
        <div className="text-center py-12">
          <p className="text-gray-500 text-lg">No persons found.</p>
          <Link
            href="/persons/create"
            className="mt-4 inline-block bg-blue-600 text-white px-6 py-2 rounded-lg hover:bg-blue-700"
          >
            Create First Person
          </Link>
        </div>
      ) : (
        <div className="overflow-x-auto shadow-lg rounded-lg">
          <table className="min-w-full bg-white">
            <thead className="bg-gray-100">
              <tr>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  ID
                </th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  First Name
                </th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  Last Name
                </th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  Age
                </th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  Created At
                </th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  Actions
                </th>
              </tr>
            </thead>
            <tbody>
              {persons.map((person) => (
                <tr key={person.personId} className="hover:bg-gray-50">
                  <td className="px-6 py-4 whitespace-nowrap text-sm font-medium text-gray-900">
                    {person.personId}
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                    {person.firstName}
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                    {person.lastName}
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                    {person.age}
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                    {new Date(person.createdAt).toLocaleDateString()}
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap text-sm font-medium">
                    <button
                      onClick={() => handleEdit(person.personId)}
                      className="text-indigo-600 hover:text-indigo-900 mr-4"
                    >
                      Edit
                    </button>
                    <button
                      onClick={() => handleDelete(person.personId)}
                      className="text-red-600 hover:text-red-900"
                    >
                      Delete
                    </button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
    </div>
  );
}