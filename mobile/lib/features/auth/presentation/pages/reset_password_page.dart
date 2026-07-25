import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import '../../../../core/constants/route_paths.dart';
import '../providers/auth_providers.dart';
import '../widgets/primary_button.dart';

class ResetPasswordPage extends ConsumerStatefulWidget {
  const ResetPasswordPage({super.key});
  @override
  ConsumerState<ResetPasswordPage> createState() => _ResetPasswordPageState();
}

class _ResetPasswordPageState extends ConsumerState<ResetPasswordPage> {
  final _formKey = GlobalKey<FormState>();
  final _tokenController = TextEditingController();
  final _passwordController = TextEditingController();
  final _confirmPasswordController = TextEditingController();

  @override
  void dispose() {
    _tokenController.dispose();
    _passwordController.dispose();
    _confirmPasswordController.dispose();
    super.dispose();
  }

  String? _validateToken(String? value) =>
      value == null || value.trim().isEmpty
      ? 'Sıfırlama kodu zorunludur.'
      : null;

  String? _validatePassword(String? value) =>
      value == null || value.length < 8
      ? 'Şifre en az 8 karakter olmalı.'
      : null;

  String? _validateConfirmPassword(String? value) =>
      value != _passwordController.text ? 'Şifreler eşleşmiyor.' : null;

  @override
  Widget build(BuildContext context) {
    final state = ref.watch(resetPasswordControllerProvider);
    ref.listen(resetPasswordControllerProvider, (previous, next) {
      if (next.failure != null && next.failure != previous?.failure) {
        ScaffoldMessenger.of(
          context,
        ).showSnackBar(SnackBar(content: Text(next.failure!.message)));
      }
      if (next.isSuccess && previous?.isSuccess != true) {
        // Web'deki (`reset-password-form.tsx`) ifadeyle birebir aynı.
        ScaffoldMessenger.of(context).showSnackBar(
          const SnackBar(content: Text('Şifreniz güncellendi.')),
        );
        context.go(RoutePaths.login);
      }
    });
    return Scaffold(
      appBar: AppBar(title: const Text('Şifreyi Sıfırla')),
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
                      'Yeni şifre belirle',
                      style: Theme.of(context).textTheme.headlineMedium,
                    ),
                    const SizedBox(height: 8),
                    const Text(
                      'E-postanıza gelen sıfırlama kodunu ve yeni şifrenizi '
                      'girin.',
                    ),
                    const SizedBox(height: 32),
                    TextFormField(
                      controller: _tokenController,
                      decoration: const InputDecoration(
                        labelText: 'Sıfırlama Kodu',
                      ),
                      validator: _validateToken,
                    ),
                    const SizedBox(height: 16),
                    TextFormField(
                      controller: _passwordController,
                      obscureText: true,
                      decoration: const InputDecoration(
                        labelText: 'Yeni Şifre (En az 8 karakter)',
                      ),
                      validator: _validatePassword,
                    ),
                    const SizedBox(height: 16),
                    TextFormField(
                      controller: _confirmPasswordController,
                      obscureText: true,
                      decoration: const InputDecoration(
                        labelText: 'Yeni Şifre Tekrar',
                      ),
                      validator: _validateConfirmPassword,
                    ),
                    const SizedBox(height: 24),
                    PrimaryButton(
                      label: 'Şifreyi Güncelle',
                      isLoading: state.isLoading,
                      onPressed: () {
                        if (_formKey.currentState!.validate()) {
                          ref
                              .read(resetPasswordControllerProvider.notifier)
                              .submit(
                                token: _tokenController.text,
                                newPassword: _passwordController.text,
                              );
                        }
                      },
                    ),
                    const SizedBox(height: 12),
                    TextButton(
                      onPressed: () => context.go(RoutePaths.login),
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
