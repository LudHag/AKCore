import type {
  UpcomingEvent,
  UpcomingEventInfo,
  UpcomingSignup,
} from "@components/Upcoming/models";

export const mockEventDetailName = "Component Test Event Detail";

export const mockEventDetailId = 5151;

export const mockSignupPersonName = "Test Signup Person";

function createMockEvent(id: number): UpcomingEvent {
  return {
    id,
    type: "Spelning",
    name: mockEventDetailName,
    place: "Test Venue",
    description: "",
    descriptionEng: "",
    internalDescription: "",
    internalDescriptionEng: "",
    fikaCollection: [],
    year: 2026,
    month: 5,
    day: "monday 05/05",
    dayDate: "2026-05-05",
    dayInMonth: 5,
    halanTime: "18:00",
    thereTime: "",
    startsTime: "19:00",
    playDuration: "",
    stand: "",
    secret: false,
    signupState: null,
    coming: 0,
    notComing: 0,
    disabled: false,
  };
}

export function createMockSignup(
  overrides: Partial<UpcomingSignup> = {},
): UpcomingSignup {
  return {
    id: 1,
    person: "test-person",
    personId: "test-person-id",
    personName: mockSignupPersonName,
    where: "Hålan",
    car: false,
    instrument: true,
    instrumentName: "Flöjt",
    otherInstruments: "",
    comment: "",
    signupTime: "2026-05-01T12:00:00",
    ...overrides,
  };
}

export function createEventDataPayload(
  overrides: Partial<UpcomingEventInfo> = {},
): UpcomingEventInfo {
  const event = createMockEvent(mockEventDetailId);
  return {
    where: null,
    car: false,
    instrument: true,
    comment: "",
    isNintendo: false,
    isPassed: false,
    members: [],
    event,
    signups: [],
    availableInstruments: ["Flute"],
    selectedInstrument: "Flute",
    ...overrides,
  };
}
