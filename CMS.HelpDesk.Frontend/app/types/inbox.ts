export interface InboxSender {
  id: number;
  name: string;
  email: string;
  location: string;
}

export interface InboxNotification {
  id: number;
  unread?: boolean;
  sender: InboxSender;
  title: string;
  message: string;
  createdAt: string;
}
