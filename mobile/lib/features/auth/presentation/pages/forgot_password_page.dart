import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import '../../../../core/constants/route_paths.dart';
import '../providers/auth_providers.dart';
import '../widgets/primary_button.dart';

class ForgotPasswordPage extends ConsumerStatefulWidget {
  const ForgotPasswordPage({super.key});
  @override
  ConsumerState<ForgotPasswordPage> createState() =>
      _ForgotPasswordPageState();
}

class _ForgotPasswordPageState extends ConsumerState<ForgotPasswordPage> {
  final _formKey = GlobalKey<FormState>();
  final _emailController = TextEditingController();

  @override
  void dispose() {
    _emailController.dispose();
    super.dispose();
  }

  String? _validateEmail(String? value) {
    final trimmed = value?.trim() ?? '';
    if (trimmed.isEmpty) return 'E-posta adresi zorunludur.';
    if (!trimmed.contains('@')) return 'Geçerli bir e-posta girin.';
    return null;
  }

  @override
  Widget build(BuildContext context) {
    final state = ref.watch(forgotPasswordControllerProvider);
    ref.listen(forgotPasswordControllerProvider, (previous, next) {
      if (next.failure != null && next.failure != previous?.failure) {
        ScaffoldMessenger.of(
          context,
        ).showSnackBar(SnackBar(content: Text(next.failure!.message)));
      }
      if (next.isSuccess && previous?.isSuccess != true) {
        // Backend, e-posta kayıtlı olsun olmasın her zaman aynı yanıtı
        // döner (anti-enumeration — bkz. `ForgotPasswordCommandHandler`);
        // mesaj web'deki (`forgot-password-form.tsx`) ifadeyle birebir
        // aynı olacak şekilde bunu yansıtır, hesabın var olduğunu ima
        // etmez.
        ScaffoldMessenger.of(context).showSnackBar(
          const SnackBar(
            content: Text(
              'Eğer bu e-posta adresine kayıtlı bir hesap varsa, şifre '
              'sıfırlama bağlantısı gönderildi.',
            ),
          ),
        );
      }
    });
    return Scaffold(
      appBar: AppBar(title: const Text('Şifremi Unuttum')),
      body: SafeArea(
        child: Center(
          child: SingleChildScrollView(
            padding: const EdgeInsets.all(24),
            child: ConstrainedBox(
              constraints: const BoxConstraints(maxWidth: 420),
              child: Form(
                key: _formKey,
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.stretch,
                  children: [
                    Text(
                      'Şifre sıfırlama',
                      style: Theme.of(context).textTheme.headlineMedium,
                    ),
                    const SizedBox(height: 8),
                    const Text(
                      'Hesabınıza kayıtlı e-posta adresinizi girin, size bir '
                      'şifre sıfırlama bağlantısı gönderelim.',
                    ),
                    const SizedBox(height: 32),
                    TextFormField(
                      controller: _emailController,
                      keyboardType: TextInputType.emailAddress,
                      decoration: const InputDecoration(labelText: 'E-posta'),
                      validator: _validateEmail,
                    ),
                    const SizedBox(height: 24),
                    PrimaryButton(
                      label: 'Gönder',
                      isLoading: state.isLoading,
                      onPressed: () {
                        if (_formKey.currentState!.validate()) {
                          ref
                              .read(forgotPasswordControllerProvider.notifier)
                              .submit(_emailController.text);
                        }
                      },
                    ),
                    const SizedBox(height: 12),
                    TextButton(
                      onPressed: () => context.push(RoutePaths.resetPassword),
                      child: const Text('Sıfırlama kodum var'),
                    ),
                    TextButton(
                      onPressed: () => context.pop(),
                      child: const Text('Giriş ekranına dön'),
                    ),
                  ],
                ),
              ),
            ),
          ),
        ),
      ),
    );
  }
}
