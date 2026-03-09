import { authService } from '@/lib/api/authService';

/** Creates a minimal fake JWT with the given payload (ASCII-safe values only). */
function createFakeJwt(payload: Record<string, unknown>): string {
  const encode = (obj: Record<string, unknown>) =>
    btoa(JSON.stringify(obj))
      .replace(/=/g, '')
      .replace(/\+/g, '-')
      .replace(/\//g, '_');
  const header = encode({ alg: 'HS256', typ: 'JWT' });
  const body = encode(payload);
  return `${header}.${body}.fakesig`;
}

const ROLE_CLAIM =
  'http://schemas.microsoft.com/ws/2008/06/identity/claims/role';

describe('authService', () => {
  beforeEach(() => {
    localStorage.clear();
  });

  // ── isAuthenticated ─────────────────────────────────────────────────────────
  describe('isAuthenticated', () => {
    it('returns false when no accessToken is stored', () => {
      expect(authService.isAuthenticated()).toBe(false);
    });

    it('returns true when an accessToken is present', () => {
      localStorage.setItem('accessToken', 'sometoken');
      expect(authService.isAuthenticated()).toBe(true);
    });
  });

  // ── getAccessToken ───────────────────────────────────────────────────────────
  describe('getAccessToken', () => {
    it('returns null when no token is stored', () => {
      expect(authService.getAccessToken()).toBeNull();
    });

    it('returns the stored access token', () => {
      localStorage.setItem('accessToken', 'mytoken');
      expect(authService.getAccessToken()).toBe('mytoken');
    });
  });

  // ── getRole ──────────────────────────────────────────────────────────────────
  describe('getRole', () => {
    it('returns null when no token is stored', () => {
      expect(authService.getRole()).toBeNull();
    });

    it('extracts the Admin role from a valid JWT', () => {
      const token = createFakeJwt({ [ROLE_CLAIM]: 'Admin' });
      localStorage.setItem('accessToken', token);
      expect(authService.getRole()).toBe('Admin');
    });

    it('extracts the User role from a valid JWT', () => {
      const token = createFakeJwt({ [ROLE_CLAIM]: 'User' });
      localStorage.setItem('accessToken', token);
      expect(authService.getRole()).toBe('User');
    });

    it('returns null when the JWT has no role claim', () => {
      const token = createFakeJwt({ sub: '123' });
      localStorage.setItem('accessToken', token);
      expect(authService.getRole()).toBeNull();
    });

    it('returns null for a malformed token', () => {
      localStorage.setItem('accessToken', 'not.a.valid.token.at.all');
      expect(authService.getRole()).toBeNull();
    });
  });

  // ── logout ───────────────────────────────────────────────────────────────────
  describe('logout', () => {
    it('removes accessToken and refreshToken from localStorage', () => {
      localStorage.setItem('accessToken', 'token');
      localStorage.setItem('refreshToken', 'refresh');
      authService.logout();
      expect(localStorage.getItem('accessToken')).toBeNull();
      expect(localStorage.getItem('refreshToken')).toBeNull();
    });

    it('does not throw when localStorage is already empty', () => {
      expect(() => authService.logout()).not.toThrow();
    });
  });
});
