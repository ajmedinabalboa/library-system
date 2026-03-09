import type { Metadata } from 'next';
import { Roboto } from 'next/font/google';
import { ThemeRegistry } from '@/components/providers/ThemeRegistry';
import { AuthProvider } from '@/lib/context/AuthContext';
import { SnackbarProvider } from '@/lib/context/SnackbarContext';

const roboto = Roboto({
  subsets: ['latin'],
  weight: ['300', '400', '500', '700'],
  display: 'swap',
});

export const metadata: Metadata = {
  title: 'Library System',
  description: 'Digital Library Management System',
};

export default function RootLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  return (
    <html lang="en">
      <body className={roboto.className}>
        <ThemeRegistry>
          <AuthProvider>
            <SnackbarProvider>{children}</SnackbarProvider>
          </AuthProvider>
        </ThemeRegistry>
      </body>
    </html>
  );
}
