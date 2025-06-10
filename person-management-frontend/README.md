# Person Management Frontend

A modern web application for managing person records, built with Next.js 13+ and TypeScript.

## Features

- View list of persons
- Create new person records
- Edit existing person records
- Delete person records
- Form validation
- Responsive design
- TypeScript support
- Tailwind CSS styling

## Tech Stack

- Next.js 13+ (App Router)
- TypeScript
- Tailwind CSS
- ESLint
- PostCSS

## Getting Started

1. Clone the repository
2. Install dependencies:
   ```bash
   npm install
   ```
3. Create a `.env.local` file in the root directory with the following content:
   ```
   NEXT_PUBLIC_API_URL=http://localhost:3001/api
   ```
4. Run the development server:
   ```bash
   npm run dev
   ```
5. Open [http://localhost:3000](http://localhost:3000) in your browser

## Project Structure

```
├── app/                              # App Router (Next.js 13+)
│   ├── layout.tsx                    # Root layout
│   ├── page.tsx                      # Home page
│   ├── globals.css                   # Global styles
│   └── persons/
│       ├── page.tsx                  # List all persons
│       ├── create/
│       │   └── page.tsx              # Create new person
│       └── edit/
│           └── [id]/
│               └── page.tsx          # Edit existing person
│
├── components/                       # Reusable components
│   ├── PersonForm.tsx               # Shared form component
│   ├── PersonTable.tsx              # Table component
│   └── Loading.tsx                  # Loading spinner
│
├── services/                        # API service layer
│   └── personService.ts             # Person API calls
│
├── types/                           # TypeScript types
│   └── person.ts                    # Person interfaces
│
├── utils/                           # Utility functions
│   └── validation.ts                # Form validation helpers
```

## Development

- `npm run dev` - Start development server
- `npm run build` - Build for production
- `npm run start` - Start production server
- `npm run lint` - Run ESLint
- `npm run type-check` - Run TypeScript type checking

## Contributing

1. Fork the repository
2. Create your feature branch
3. Commit your changes
4. Push to the branch
5. Create a new Pull Request
