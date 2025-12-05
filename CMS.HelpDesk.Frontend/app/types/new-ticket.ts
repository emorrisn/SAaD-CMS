export type Suggestion = {
  id: string;
  title: string;
  summary: string;
  tags: string[];
  category?: string;
};

export type Customer = {
  firstName: string;
  lastName: string;
  phone: string;
};

export type MenuItem = { label: string; value: string };

export type FormError = { path: string; message: string };
