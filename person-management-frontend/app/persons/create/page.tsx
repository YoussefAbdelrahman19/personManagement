'use client';

import { useState } from 'react';
import { useRouter } from 'next/navigation';
import PersonForm from '@/components/PersonForm';
import { createPerson } from '@/services/personService';
import { validatePerson } from '@/utils/validation';

export default function CreatePersonPage() {
  const router = useRouter();
  const [error, setError] = useState<string | null>(null);
  const [isSubmitting, setIsSubmitting] = useState(false);

  const handleSubmit = async (data: any) => {
    const validation = validatePerson(data);
    if (!validation.isValid) {
      setError(Object.values(validation.errors)[0]);
      return;
    }

    setIsSubmitting(true);
    try {
      await createPerson(data);
      router.push('/persons');
    } catch (err) {
      setError('Failed to create person');
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <div>
      <h1 className="text-2xl font-bold mb-6">Create New Person</h1>
      {error && <div className="text-red-600 mb-4">{error}</div>}
      <PersonForm onSubmit={handleSubmit} isSubmitting={isSubmitting} />
    </div>
  );
} 