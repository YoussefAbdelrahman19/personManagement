export const validateEmail = (email: string): boolean => {
  const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
  return emailRegex.test(email);
};

export const validatePhone = (phone: string): boolean => {
  const phoneRegex = /^\+?[\d\s-]{10,}$/;
  return phoneRegex.test(phone);
};

export const validateName = (name: string): boolean => {
  return name.trim().length >= 2;
};

export const validatePerson = (person: {
  name: string;
  email: string;
  phone: string;
}): { isValid: boolean; errors: Record<string, string> } => {
  const errors: Record<string, string> = {};

  if (!validateName(person.name)) {
    errors.name = 'Name must be at least 2 characters long';
  }

  if (!validateEmail(person.email)) {
    errors.email = 'Please enter a valid email address';
  }

  if (!validatePhone(person.phone)) {
    errors.phone = 'Please enter a valid phone number';
  }

  return {
    isValid: Object.keys(errors).length === 0,
    errors,
  };
}; 